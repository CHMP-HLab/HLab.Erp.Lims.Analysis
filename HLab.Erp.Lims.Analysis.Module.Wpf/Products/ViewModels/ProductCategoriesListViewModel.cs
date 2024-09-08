using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

public class ProductCategoriesListViewModel : Core.EntityLists.EntityListViewModel<ProductCategory>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param/products";
    }

    public ProductCategoriesListViewModel(Injector i) : base(i, c => c
        // TODO .AddAllowed()
        // TODO .DeleteAllowed()
        .Column("Name")
        .Header("{Name}")
        .Width(250)
        .Localize(e => e.Name)
        .Icon(p => p.IconPath)
        .Link(e => e.Name)
        .OrderByAsc(0)
        .Filter()
    )
    {
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

}