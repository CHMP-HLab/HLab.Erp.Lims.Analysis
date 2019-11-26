using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
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
        public async Task LoadResult(SampleTestResult target=null)
        {
            await FormHelper.ExtractCode(Model.Code).ConfigureAwait(true);
            await FormHelper.LoadForm().ConfigureAwait(true);

            FormHelper.LoadValues(Model.Values);
            if (target!=null) FormHelper.LoadValues(target.Values);

            Model.Values = FormHelper.GetPackedValues(FormHelper.TestValueLevel.Test);
            
            if(target!=null)
            target.Values = FormHelper.GetPackedValues(FormHelper.TestValueLevel.Result);

            FormHelper.Form.Traitement(null,null);

            FormHelper.SetFormMode(TestFormMode.ReadOnly);
        }


        public ListTestResultViewModel Results => _results.Get();
        private readonly IProperty<ListTestResultViewModel> _results = H.Property<ListTestResultViewModel>(c => c
            .On(e => e.Model)
            .Set(e =>
            {
                var vm =  e._getResults(e.Model.Id);
                vm.SetSelectAction(async r => await e.LoadResult(r as SampleTestResult));
                return vm;
            })
        );

        public ICommand ViewSpecificationsCommand {get;} = H.Command(c => c
            .Action(async e => await e.LoadResult())
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
            Results.List.Update();
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
