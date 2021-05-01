using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public ProductsListPopupViewModel() : base(c => c
                .Column()
                    .Header("{Inn}")
                    .Width(200)
                    .Content(p => p.Inn)
                    .Filter<TextFilter>()
                .Column()
                    .Header("{Dose}")
                    .Width(100)
                    .Content(p => p.Dose)
                    .Filter<TextFilter>()
                .FormColumn( p => p.Form,p => p.FormId)
        )
        {

        }
    }
}