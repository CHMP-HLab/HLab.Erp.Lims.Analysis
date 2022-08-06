
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers;

public class ManufacturersListViewModel : Core.EntityLists.EntityListViewModel<Manufacturer>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }

    readonly IAclService _acl;

    public ManufacturersListViewModel(IAclService acl, Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}")
        .Width(250).Link(e => e.Name)
        .Filter()
        .Header("{Name}")


        .Column(e => e.Country, "Country").Mvvm().Width(150)
    )
    {
        _acl = acl;
    }

    protected override bool CanExecuteAdd(Action<string> errorAction) => _acl.IsGranted(errorAction, AnalysisRights.AnalysisManufacturerCreate);
    protected override bool CanExecuteDelete(Manufacturer manufacturer, Action<string> errorAction) => _acl.IsGranted(errorAction, AnalysisRights.AnalysisManufacturerCreate);

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

}