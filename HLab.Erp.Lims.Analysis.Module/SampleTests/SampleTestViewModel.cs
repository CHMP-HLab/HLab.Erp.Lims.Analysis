using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleTestViewModelDesign : SampleTestViewModel, IViewModelDesign
    {

    }

    public class SampleTestViewModel : EntityViewModel<SampleTestViewModel,SampleTest>
    {
        [Import]
        private readonly Func<int, ListTestResultViewModel> _getResults;

        [Import]
        public IAclService Acl {get;}

        [Import]
        private IDataService _data;

        public FormHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c.Default(new FormHelper()));
        public async Task LoadResultAsync(SampleTestResult target=null)
        {
            await FormHelper.Load(Model,target).ConfigureAwait(true);
            FormHelper.SetFormMode(target == null ? TestFormMode.Specification : TestFormMode.Capture);
        }

        public SampleTestWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestWorkflow> _workflow = H.Property<SampleTestWorkflow>(c => c
            .On(e => e.Model)
            .Set(vm => new SampleTestWorkflow(vm.Model))
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

        public ICommand ViewSpecificationsCommand {get;} = H.Command(c => c
            .Action(async e => await e.LoadResultAsync())
        );

        public ICommand AddResultCommand { get; } = H.Command(c => c
            //.CanExecute(e => e.Acl.IsGranted(AnalysisRights.AnalysisAddResult))
            .Action((e,t) => e.AddResult(e.Results.Selected))
        );
        public ICommand SelectResultCommand { get; } = H.Command(c => c
            //.CanExecute(e => e.Results.List.Selected.Validation == 3)
            .Action((e,t) => e.SelectResult(e.Results.Selected))
        );

        private void SelectResult(SampleTestResult result)
        {
            if (result==null)
                Model.Result = null;

            Model.Result = result;
            //Results.List.Clear();
            Results.List.Refresh();
        }

        private void AddResult(SampleTestResult previous)
        {
            var test = _data.Add<SampleTestResult>(r =>
            {
                r.SampleTest = Model;
                r.UserId = Model.UserId;
            });

            if (test != null)
                Results.List.Update();
        }

        public string Title => Model.Sample?.Reference + "\n" + Model.TestName + "\n" + Model.Description;
    }
}
