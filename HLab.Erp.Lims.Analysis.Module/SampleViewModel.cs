using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    class SampleViewModel : EntityViewModel<SampleViewModel,Sample>
    {
        public SampleViewModel()
        { }
        public SampleViewModel(Func<int, ListSampleAssayViewModel> getAssays)
        {
            _getAssays = getAssays;
            Packagings.Update();
        }
        public string Title => Model.Customer.Name + "\n" + Model.Product.Caption + "\n" + Model.Ref;

        [Import] public ObservableQuery<Packaging> Packagings { get; }

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

        [Import]
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

    class SampleViewModelDesign
    {
        public Sample Model { get; } = Sample.DesignModel;
    }
}
