using System;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

public class ProductsListViewModel : Core.EntityLists.EntityListViewModel<Product>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader { }

    readonly IAclService _acl;

    public ProductsListViewModel(IAclService acl, Injector i) : base(i, c => c
        .Column("Category")
        .Header("{Category}")
        .Width(100)
        .Content(e => e.Category == null ? "" : e.Category.Name)
        .OrderBy(e => e.Category?.Name)

        .Column("Name")
        .Header("{Name}")
        .IconPath("Icons/Entities/Products/Inn")
        .Width(300)
        .Link(e => e.Name)
        .OrderBy(e => e.Name)
        .Filter()

        .Column("Variant")
        .Header("{Dose}")
        .IconPath("Icons/Entities/Products/Dose")
        .Width(200)
        .Link(e => e.Variant)
        .OrderBy(e => e.Variant)
        .Filter()

        .FormColumn(e => e.Form)
    )
    {
        _acl = acl;
    }

    protected override bool CanExecuteAdd(Action<string> errorAction) => _acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
    protected override bool CanExecuteDelete(Product product, Action<string> errorAction) => _acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
    protected override bool CanExecuteImport(Action<string> errorAction) => _acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
    protected override bool CanExecuteExport(Action<string> errorAction) => true;

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}