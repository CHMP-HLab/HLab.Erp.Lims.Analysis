using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleAssays;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    class SampleViewModel : EntityViewModel<SampleViewModel,Sample>
    {
        public SampleViewModel()
        { }
        [Import] public SampleViewModel(Func<int, ListSampleAssayViewModel> getAssays, ObservableQuery<Packaging> packagings)
        {
            _getAssays = getAssays;
            Packagings = packagings;
            Packagings.Update();
        }
        public string Title => Model.Customer.Name + "\n" + Model.Product.Caption + "\n" + Model.Ref;

        public ObservableQuery<Packaging> Packagings { get; }

        [TriggerOn(nameof(Packagings),"Item","Secondary")]
        public IObservableFilter<Packaging> PrimaryPackagingList { get; }
            = H.Filter<Packaging>((e, f) => f
                .AddFilter(p => p.Secondary == false)
                .Link(() => e.Packagings));

        public IObservableFilter<Packaging> SecondaryPackagingList { get; }
            = H.Filter<Packaging>((e, f) => f
                .AddFilter(p => p.Secondary == true)
                .Link(() => e.Packagings)
            );

        
        private readonly Func<int, ListSampleAssayViewModel> _getAssays;

        public ListSampleAssayViewModel Assays => _assays.Get();
        private readonly IProperty<ListSampleAssayViewModel> _assays = H.Property<ListSampleAssayViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getAssays(e.Model.Id))
        );

        public SampleWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleWorkflow> _workflow = H.Property<SampleWorkflow>(c => c
            .On(e => e.Model)
            .Set(vm => new SampleWorkflow(vm.Model))
        );

                                                       
    }

    class SampleViewModelDesign : SampleViewModel, IViewModelDesign
    {
        public new Sample Model { get; } = Sample.DesignModel;
    }
}
