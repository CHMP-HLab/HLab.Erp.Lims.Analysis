using System;
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

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleTestViewModelDesign : SampleTestViewModel, IViewModelDesign
    {

    }

    public class SampleTestViewModel : EntityViewModel<SampleTestViewModel,SampleTest>, IMvvmContextProvider
    {
        [Import] private readonly Func<int, ListTestResultViewModel> _getResults;

        [Import] public IErpServices Erp { get; }

        [Import]
        private IDataService _data;

        public FormHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
            .Set(e => e._getFormHelper()));

        [Import] private Func<FormHelper> _getFormHelper;

        public async Task LoadResultAsync(SampleTestResult target=null)
        {
            await FormHelper.LoadAsync(Model,target).ConfigureAwait(true);

            var state = Workflow.CurrentState;

            if (state == SampleTestWorkflow.Specifications) 
                FormHelper.Mode = TestFormMode.Specification;
            //else if (state == SampleTestWorkflow.Running) 
            //    FormHelper.Mode = TestFormMode.Capture;
            else 
                FormHelper.Mode = TestFormMode.ReadOnly;
        }

        public SampleTestWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestWorkflow> _workflow = H.Property<SampleTestWorkflow>(c => c
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Set(vm => vm._getSampleTestWorkflow(vm.Model,vm.Locker))
        );
        [Import] private Func<SampleTest, DataLocker<SampleTest>, SampleTestWorkflow> _getSampleTestWorkflow;

        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .On(e => e.EditMode)
            .Set(e => !e.EditMode)
        );

        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleTestWorkflow.Specifications
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisMonographSign)
            )
        );
        public bool ScheduleEditMode => _scheduleEditMode.Get();
        private readonly IProperty<bool> _scheduleEditMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleTestWorkflow.Scheduling
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisSchedule)
            )
        );

        public bool ResultMode => _resultMode.Get();
        private IProperty<bool> _resultMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleTestWorkflow.Running
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
        );
        public bool FormHelperIsActive => _formHelperIsActive.Get();
        private IProperty<bool> _formHelperIsActive = H.Property<bool>(c => c
        .On(e => e.EditMode)
        .On(e => e.ResultMode)
        .Set(e => e.EditMode || e.ResultMode)
        );

        public ListTestResultViewModel Results => _results.Get();
        private readonly IProperty<ListTestResultViewModel> _results = H.Property<ListTestResultViewModel>(c => c
            .On(e => e.Model)
            .Set(e =>
            {
                var vm =  e._getResults(e.Model.Id);
                vm.SetSelectAction(async r => await e.LoadResultAsync(r as SampleTestResult).ConfigureAwait(false));
                return vm;
            })
        );

        private readonly IProperty<bool> _ = H.Property<bool>(c => c
            .On(e => e.Model)
            .OnNotNull(e => e.Workflow)
            .OnNotNull(e => e.Results)
            .Do(async e => await e.LoadResultAsync(e.Results.Selected??e.Model.Result))
        );

        public ICommand ViewSpecificationsCommand {get;} = H.Command(c => c
            .Action(async e => await e.LoadResultAsync())
        );

        public ICommand AddResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addResultCanExecute())
            .Action((e,t) => e.AddResult(e.Results.Selected))
            .On(e => e.Workflow.CurrentState).CheckCanExecute()
        );
        public ICommand DeleteResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._deleteResultCanExecute())
            .Action((e,t) => e.DeleteResult(e.Results.Selected))
            .On(e => e.Workflow.CurrentState)
            .On(e => e.Results.Selected.Stage)
            .On(e => e.Model.Result)
            .CheckCanExecute()
        );

        private bool _addResultCanExecute()
        {
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentState != SampleTestWorkflow.Running) return false;

            return true;
        }

        private bool _deleteResultCanExecute()
        {
            if(Workflow==null) return false;
            if(Results?.Selected==null) return false;
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentState != SampleTestWorkflow.Running) return false;
            if(Results.Selected.Stage!="Running") return false;
            if(Model.Result==null) return true;
            if(Model.Result.Id == Results.Selected.Id) return false;
            return true;
        }

        public ICommand SelectResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e.Results?.List?.Selected?.Stage == SampleTestResultWorkflow.Validated.Name)
            .Action(async (e,t) => await e.SelectResult(e.Results.Selected))
            .On(e => e.Results.List.Selected).CheckCanExecute()
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

        public ITestHelper TestHelper => _testHelper.Get();
        private readonly IProperty<ITestHelper> _testHelper = H.Property<ITestHelper>(c => c
            .On(e => e.FormHelper.Form.Test)
            .NotNull(e => e.FormHelper?.Form)
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
                case "Description":
                    if(TestHelper?.Description!=null)
                        Model.Description = TestHelper.Description;
                    break;
                case "Specifications":
                    if(TestHelper?.Specifications!=null)
                        Model.Specification = TestHelper.Specifications;
                    break;
                case "Conformity":
                    if(TestHelper?.Conformity!=null)
                        Model.Conform = TestHelper.Conformity;
                    break;
                //case "SpecificationsDone":
                //    Model.SpecificationsDone = TestHelper.SpecificationsDone;
                //    break;
            }
        }
        
        public override string Title => _title.Get();
        private IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Sample.Reference)
            .Set(e => e.Model.Sample?.Reference)
        );

        public string SubTitle => _subTitle.Get();
        private IProperty<string> _subTitle = H.Property<string>(c => c
            .On(e => e.Model.TestName)
            .On(e => e.Model.Description)
            .Set(e => e.Model.TestName + "\n" + e.Model.Description)
        );

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
            ctx.AddCreator<SampleTestResultViewModel>(vm => vm.Parent = this);
        }
    }
}
