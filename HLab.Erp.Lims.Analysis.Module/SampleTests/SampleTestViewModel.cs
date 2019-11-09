using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
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

        public ListTestResultViewModel Results => _results.Get();
        private readonly IProperty<ListTestResultViewModel> _results = H.Property<ListTestResultViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getResults(e.Model.Id))
        );

        public string Title => Model.Sample.Ref + "\n" + Model.TestName + "\n" + Model.Description;
    }
}
