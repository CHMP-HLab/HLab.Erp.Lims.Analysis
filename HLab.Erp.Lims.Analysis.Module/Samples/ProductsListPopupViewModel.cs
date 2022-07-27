using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class ProductsListPopupViewModel : Core.EntityLists.EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public ProductsListPopupViewModel(Injector i) : base(i, c => c
                    .Column("Name")
                        .Header("{Name}")
                        .Width(200)
                        .Link(p => p.Name)
                        .Filter()

                    .Column("Variant")
                        .Header("{Variant}")
                        .Width(100)
                        .Link(p => p.Variant)
                        .Filter()

                .FormColumn( p => p.Form)
        )
        {

        }
    }
}