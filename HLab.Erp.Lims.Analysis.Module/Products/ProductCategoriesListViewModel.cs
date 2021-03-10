using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Icons.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductCategoriesListViewModel : EntityListViewModel<ProductCategory>, IMvvmContextProvider
    {
        public ProductCategoriesListViewModel()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                    .Column
                        .Header("{Name}")
                        .Width(150)
                        .Content(s => s.Name)
                        .Icon(s => s.IconPath)
                
                );
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f.Title("{Name}")
                    .Link(List,e => e.Name));
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
