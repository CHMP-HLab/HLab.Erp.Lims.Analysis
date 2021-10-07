using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
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
            .Header("{Name}").Localize()
            .Width(250).Content(e => e.Name)
                    .Localize()
                    .Icon(p => p.IconPath)
                    .Link(e => e.Name)
                        .Filter()
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }

}
