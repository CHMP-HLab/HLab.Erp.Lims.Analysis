using System;
using HLab.Core.Annotations;
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
    public class Bootloader : NestedBootloader
    {
        public override void Load(IBootContext bootstrapper)
        {
            Menu.RegisterMenu("param/products", "{Products}", null, "Icons/Entities/Product");
            base.Load(bootstrapper);
        }
    }

    readonly IAclService _acl;

    public ProductsListViewModel(IAclService acl, Injector i) : base(i, c => c
        .ColumnListable(e => e.Category)
        //.Column("Category")
        //.Header("{Category}")
        //.Width(100)
        //.Content(e => e.Category.Name).Localize()
        .OrderBy(e => e.Category?.Name)

        .Column("Name")
        .Header("{Name}")
        .IconPath("Icons/Entities/Inn")
        .Width(300)
        .Localize(e => e.Name)
        .Link(e => e.Name)
        .OrderBy(e => e.Name)
        .OrderByAsc()
        .Filter()

        .Column("Variant")
        .Header("{Dose}")
        .IconPath("Icons/Entities/Products/Dose")
        .Width(200)
        .Localize(e => e.Variant)
        .Link(e => e.Variant)
        .OrderBy(e => e.Variant)
        .OrderByAsc(1)
        .Filter()

        .ColumnListable(e => e.Form)

        .AddProperty("IsValid",p=>true,p => true)
        //.FormColumn(e)
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