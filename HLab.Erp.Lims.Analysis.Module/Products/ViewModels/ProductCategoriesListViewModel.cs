
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductCategoriesListViewModel : EntityListViewModel<ProductCategory>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
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
