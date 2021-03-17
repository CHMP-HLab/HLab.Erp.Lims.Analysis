using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using Outils;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{

    using H = H<SampleTestViewModel>;

    public class SampleTestViewModelDesign : SampleTestViewModel, IViewModelDesign
    {

    }

    public class SampleTestViewModel : EntityViewModel<SampleTest>, IMvvmContextProvider
    {
        public SampleTestViewModel() => H.Initialize(this);

        [Import] private readonly Func<SampleTest, ListTestResultViewModel> _getResults;

        [Import] public IErpServices Erp { get; }

        [Import]
        private IDataService _data;

        public FormTestClassHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormTestClassHelper> _formHelper = H.Property<FormTestClassHelper>(c => c
            .Set(e => e._getFormHelper()));

        [Import] private Func<FormTestClassHelper> _getFormHelper;

        public async Task LoadResultAsync(SampleTestResult target=null)
        {
            await FormHelper.LoadAsync(Model,target).ConfigureAwait(true);

            var state = Workflow.CurrentStage;

            if (state == SampleTestWorkflow.Specifications) 
                FormHelper.Mode = TestFormMode.Specification;
            //else if (state == SampleTestWorkflow.Running) 
            //    FormHelper.Mode = TestFormMode.Capture;
            else 
                FormHelper.Mode = TestFormMode.ReadOnly;
        }

        public SampleTestWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestWorkflow> _workflow = H.Property<SampleTestWorkflow>(c => c
            .NotNull(e => e.Locker)
            .Set(vm => vm._getSampleTestWorkflow(vm.Model,vm.Locker))
            .On(e => e.Model)
            .On(e => e.Locker)
            .Update()
        );

        [Import] private Func<SampleTest, DataLocker<SampleTest>, SampleTestWorkflow> _getSampleTestWorkflow;

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .Set(e => !e.EditMode)
            .On(e => e.EditMode).Update()
        );

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => {
                e.FormHelper.SetFormMode(e.FormHelper.Mode);
                return e.Locker.IsActive
                                && e.Workflow.CurrentStage == SampleTestWorkflow.Specifications
                                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisMonographSign);
            })
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );
        public bool ScheduleEditMode => _scheduleEditMode.Get();
        private readonly IProperty<bool> _scheduleEditMode = H.Property<bool>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentStage == SampleTestWorkflow.Scheduling
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisSchedule)
            )
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage).Update()
        );

        public bool ResultMode => _resultMode.Get();
        private IProperty<bool> _resultMode = H.Property<bool>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentStage == SampleTestWorkflow.Running
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisResultEnter))
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
        .Update()
         
        );
        public bool FormHelperIsActive => _formHelperIsActive.Get();
        private readonly IProperty<bool> _formHelperIsActive = H.Property<bool>(c => c
        .Set(e => e.EditMode || e.ResultMode)
        .On(e => e.EditMode)
        .On(e => e.ResultMode).Update()
        );


        // RESULTS
        public ListTestResultViewModel Results => _results.Get();
        private readonly IProperty<ListTestResultViewModel> _results = H.Property<ListTestResultViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e =>
            {
                var vm =  e._getResults(e.Model);
                vm.SetSelectAction(async r =>
                {
                    await e.LoadResultAsync(r as SampleTestResult).ConfigureAwait(false);
                    if (e.SelectResultCommand is CommandPropertyHolder nc) nc.CheckCanExecute();
                });

                SampleTestResult selected = null;
                foreach(var result in vm.List)
                {
                    if(selected==null) selected = result;
                    if(result == e.Model.Result) selected = result;
                }
                vm.Selected = selected;
                return vm;
            })
            .On(e => e.Model)
            .Update()
        );

        private readonly ITrigger _trigger = H.Trigger(c => c
            .On(e => e.Model)
            .OnNotNull(e => e.Workflow)
            .OnNotNull(e => e.Results)
            .On(e => e.Locker.IsActive)
            .Do(async e => await e.LoadResultAsync(e.Results.Selected??e.Model.Result))
        );

        public ICommand ViewSpecificationsCommand {get;} = H.Command(c => c
            .Action(async e => await e.LoadResultAsync())
        );

        public ICommand AddResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addResultCanExecute())
            .Action((e,t) => e.AddResult(e.Results.Selected))
            .On(e => e.Workflow.CurrentStage).CheckCanExecute()
        );
        public ICommand DeleteResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._deleteResultCanExecute())
            .Action((e,t) => e.DeleteResult(e.Results.Selected))
            .On(e => e.Workflow.CurrentStage)
            .On(e => e.Results.Selected.Stage)
            .On(e => e.Results.Selected)
            .On(e => e.Model.Result)
            .CheckCanExecute()
        );

        private bool _addResultCanExecute()
        {
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentStage != SampleTestWorkflow.Running) return false;

            return true;
        }

        private bool _deleteResultCanExecute()
        {
            if(Workflow==null) return false;
            if(Results?.Selected==null) return false;
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentStage != SampleTestWorkflow.Running) return false;
            if(Results.Selected.Stage!=null && Results.Selected.Stage != SampleTestResultWorkflow.Running.Name) return false;
            if(Model.Result==null) return true;
            if(Model.Result.Id == Results.Selected.Id) return false;
            return true;
        }


        //TODO : probleme here where button not always available where conditions are ok
        public ICommand SelectResultCommand { get; } = H.Command(c => c
            .CanExecute(e => 
                e.Results?.Selected?.Stage == SampleTestResultWorkflow.Validated.Name
                && e.Model.Stage == SampleTestWorkflow.Running.Name
                && e.Locker.IsActive
                )
            .Action(async (e, t) =>
            {
                if(
                    e.Results?.Selected?.Stage == SampleTestResultWorkflow.Validated.Name
                    && e.Model.Stage == SampleTestWorkflow.Running.Name
                    && e.Locker.IsActive)
                    await e.SelectResult(e.Results.Selected);
            })
            .On(e => e.Results.Selected.Stage)
            .On(e => e.Results.Selected)
            .On(e => e.Model.Stage)
            .On(e => e.Locker.IsActive)
        
        .CheckCanExecute()
        );

        public ICommand OpenSampleCommand { get; } = H.Command(c => c
            .Action(async (e, t) =>
            {
                await e.Erp.Docs.OpenDocumentAsync(e.Model.Sample);
            })
        );
        public ICommand OpenProductCommand { get; } = H.Command(c => c
            .Action(async (e, t) =>
            {
                await e.Erp.Docs.OpenDocumentAsync(e.Model.Sample.Product);
            })
        );
        public ICommand OpenCustomerCommand { get; } = H.Command(c => c
            .Action(async (e, t) =>
            {
                await e.Erp.Docs.OpenDocumentAsync(e.Model.Sample.Customer);
            })
        );

        private async Task SelectResult(SampleTestResult result)
        {
            if(result.Stage == SampleTestResultWorkflow.Validated.Name)
            {
                Model.Result = result;
                await Results.List.RefreshAsync();
            }
        }

        private void AddResult(SampleTestResult previous)
        {
            int i = 0;

            foreach (var r in Results.List)
            {
                var n = r.Name??"";
                if (n.StartsWith("R")) n = n.Substring(1);

                if(int.TryParse(n, out var v))
                {
                    i = Math.Max(i,v);
                }
            }            
            
            var test = _data.Add<SampleTestResult>(r =>
            {
                r.Name = string.Format("R{0}",i+1);
                r.SampleTest = Model;
                r.UserId = Model.UserId;
            });

            if (test != null)
                Results.List.UpdateAsync();
        }

        private void DeleteResult(SampleTestResult result)
        {
            if(_deleteResultCanExecute())
            {
                _data.Delete(result);

            }
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>

        public ITestHelper TestHelper => _testHelper.Get();
        private readonly IProperty<ITestHelper> _testHelper = H.Property<ITestHelper>(c => c
            .On(e => e.FormHelper.Form.Test)
            .NotNull(e => e.FormHelper?.Form?.Test)
            .Do((e,f) => {
                f.Set(e.FormHelper.Form.Test);
                e.TestHelper.PropertyChanged += e.TestHelper_PropertyChanged;
                })
        );

        private void TestHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Model==null) return;
            switch(e.PropertyName)
            {
                case "TestName":
                    if(TestHelper?.TestName!=null)
                        Model.TestName = TestHelper.TestName;
                    break;
                case "Description":
                    if(TestHelper?.Description!=null)
                        Model.Description = TestHelper.Description;
                    break;
                case "Specifications":
                    if(TestHelper?.Specifications!=null)
                        Model.Specification = TestHelper.Specifications;
                    break;
                //case "Conformity":
                //    if(TestHelper?.Conformity!=null)
                //        Model.Conform = TestHelper.Conformity;
                //    break;
                //case "SpecificationsDone":
                //    Model.SpecificationsDone = TestHelper.SpecificationsDone;
                //    break;
            }
        }
        
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .Set(e => e.Model.Sample?.Reference)
            .On(e => e.Model.Sample.Reference)
        .Update()
        );

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.Model.TestName + "\n" + e.Model.Description.TrimEnd('\r','\n',' '))
            .On(e => e.Model.TestName)
            .On(e => e.Model.Description)
            .Update()
        );

        public string ConformityIconPath => _conformityIconPath.Get();

        private readonly IProperty<string> _conformityIconPath = H.Property<string>(c => c
            .Set(e =>
            {
                if (e.Model.Result == null) return "Icons/Results/ConformityTodo";
                switch(e.Model.Result.ConformityId)
                {
                    case ConformityState.Undefined : return "Icons/Validations/Error";
                    case ConformityState.NotChecked : return "Icons/Results/ConformityTodo";
                    case ConformityState.Running : return "Icons/Results/Running";
                    case ConformityState.NotConform : return "Icons/Results/ConformityKo";
                    case ConformityState.Conform : return "Icons/Results/ConformityOK";
                    case ConformityState.Invalid : return "Icons/Results/Invalidated";
                    default: return "Icons/Validations/Error";
                } 
            }   )         
            .On(e => e.Model.Result.ConformityId)
            .Update()
        );

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
            ctx.AddCreator<SampleTestResultViewModel>(vm => vm.Parent = this);
        }
    }
}
