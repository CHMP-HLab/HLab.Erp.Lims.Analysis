using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    internal class ProductCategoryViewModel: ListableEntityViewModel<ProductCategory>
    {
        public ProductCategoryViewModel(Injector i) : base(i)
        {
        }
    }
}