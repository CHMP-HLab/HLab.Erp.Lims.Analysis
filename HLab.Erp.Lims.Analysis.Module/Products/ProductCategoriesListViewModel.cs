using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductCategoriesListViewModel : EntityListViewModel<ProductCategory>, IMvvmContextProvider
    {
        public ProductCategoriesListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()
                    .Column()
                        .Header("{Name}")
                        .Width(150)
                        .Content(s => s.Name)
                        .Icon(s => s.IconPath)
                        .Filter<TextFilter>()
            
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }

}
