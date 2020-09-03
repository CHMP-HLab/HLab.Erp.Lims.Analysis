using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductCategoriesListViewModel : EntityListViewModel<ProductCategory>, IMvvmContextProvider
    {

        [Import] private readonly IErpServices _erp;


        public ProductCategoriesListViewModel()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s => new IconView { Path = s.IconPath, Width = 30 })
                .Column("{Name}", s => s.Name)
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {

                List.UpdateAsync();
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
