using System;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    class SampleViewModel : EntityViewModel<SampleViewModel,Sample>
    {
        public SampleViewModel()
        { }
        [Import] public SampleViewModel(Func<int, ListSampleTestViewModel> getTests, ObservableQuery<Packaging> packagings)
        {
            _getTests = getTests;
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

        
        private readonly Func<int, ListSampleTestViewModel> _getTests;

        public ListSampleTestViewModel Tests => _tests.Get();
        private readonly IProperty<ListSampleTestViewModel> _tests = H.Property<ListSampleTestViewModel>(c => c
            .On(e => e.Model)
            .Set(e => e._getTests(e.Model.Id))
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
