using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
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
            .Default(new FormHelper()));

        public async Task LoadResultAsync(SampleTestResult target=null)
        {
            await FormHelper.LoadAsync(Model,target).ConfigureAwait(true);

            var state = Workflow.CurrentState;

            if (state == SampleTestWorkflow.Specifications) 
                FormHelper.Mode = TestFormMode.Specification;
            else if (state == SampleTestWorkflow.Running) 
                FormHelper.Mode = TestFormMode.Capture;
            else 
                FormHelper.Mode = TestFormMode.ReadOnly;
        }

        public SampleTestWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestWorkflow> _workflow = H.Property<SampleTestWorkflow>(c => c
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Set(vm => new SampleTestWorkflow(vm.Model,vm.Locker))
        );
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
            .Do(async e => await e.LoadResultAsync(e.Results.Selected))
        );

        public ICommand ViewSpecificationsCommand {get;} = H.Command(c => c
            .Action(async e => await e.LoadResultAsync())
        );

        public ICommand AddResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._addResultCanExecute())
            .Action((e,t) => e.AddResult(e.Results.Selected))
        );
        public ICommand DeleteResultCommand { get; } = H.Command(c => c
            .CanExecute(e => e._deleteResultCanExecute())
            .Action((e,t) => e.DeleteResult(e.Results.Selected))
        );

        private bool _addResultCanExecute()
        {
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentState != SampleTestWorkflow.Running) return false;

            return true;
        }

        private bool _deleteResultCanExecute()
        {
            if(!Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(Workflow.CurrentState != SampleTestWorkflow.Running) return false;
            if(Results.Selected == null) return false;
            if(Results.Selected.Stage!="Running") return false;
            if(Model.Result.Id == Results.Selected.Id) return false;
            return true;
        }

        public ICommand SelectResultCommand { get; } = H.Command(c => c
            //.CanExecute(e => e.Results.List.Selected.Validation == 3)
            .Action(async (e,t) => await e.SelectResult(e.Results.Selected))
        );

        private async Task SelectResult(SampleTestResult result)
        {
            Model.Result = result;
            //Results.List.Clear();
            await Results.List.RefreshAsync();
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
