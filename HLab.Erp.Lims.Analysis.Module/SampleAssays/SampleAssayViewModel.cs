using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleAssays
{
    public class SampleAssayViewModelDesign : SampleAssayViewModel, IViewModelDesign
    {

    }

    public class SampleAssayViewModel : EntityViewModel<SampleAssayViewModel,SampleAssay>
    {
        [Import]
        private readonly Func<int, ListAssayResultViewModel> _getResults;

        public ListAssayResultViewModel Results => _results.Get();
        private readonly IProperty<ListAssayResultViewModel> _results = H.Property<ListAssayResultViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getResults(e.Model.Id))
        );

        public string Title => Model.Sample.Ref + "\n" + Model.AssayName + "\n" + Model.Description;
    }
}
