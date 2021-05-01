using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductsListViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader { }
        public ProductsListViewModel() : base(c => c
           .AddAllowed()
           .DeleteAllowed()
               .Column()
                   .Header("{Category}")
                   .Width(100)
                   .Content(e => e.Category?.Name)
                   .OrderBy(e => e.Category?.Name)
               .Column()
                   .Header("{Inn}")
                   .Width(300)
                   .Content(e => e.Inn)
                        .Filter<TextFilter>()
                        .IconPath("Icons/Entities/Products/Inn")
                        .Link(e => e.Inn)
               .Column()
                   .Header("{Dose}")
                   .Width(200)
                   .Content(e => e.Dose)
                        .Filter<TextFilter>()
                        .IconPath("Icons/Entities/Products/Dose")
                        .Link(e => e.Dose)

               .FormColumn( e => e.Form,e => e.FormId)
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}