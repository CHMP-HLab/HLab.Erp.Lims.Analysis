
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
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
                        .Link(p => p.Inn)
                        .Filter()

                    .Column()
                        .Header("{Dose}")
                        .Width(100)
                        .Link(p => p.Dose)
                        .Filter()

                .FormColumn( p => p.Form)
        )
        {

        }
    }
}