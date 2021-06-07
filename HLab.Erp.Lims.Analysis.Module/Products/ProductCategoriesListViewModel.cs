
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductCategoriesListViewModel : EntityListViewModel<ProductCategory>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "data/products";
        }

        public ProductCategoriesListViewModel() : base(c => c
            // TODO .AddAllowed()
            //.DeleteAllowed()
            .Column()
            .Header("{Name}")
            .Width(150)
                .Link(s => s.Name)
                .Icon(s => s.IconPath)
                    .Filter()
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }

}
