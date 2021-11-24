using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public ProductsListPopupViewModel() : base(c => c
                .Header("{Products}")
                //.AddAllowed()
                .Column("Reference")
                    .Header("{Ref}")
                    .Link(p => p.Caption)
                
                .Column("Name")
                    .Header("{Name}")
                    .Link(p => p.Name)
                    .Filter()
                    .IconPath("Icons/Entities/Products/Inn")

                .Column("Variant")
                    .Header("{Dose}")
                    .Link(p => p.Variant)
                    .Filter()
                    .IconPath("Icons/Entities/Products/Dose")
                    .Link(p => p.Variant)

                .FormColumn( p => p.Form)

        )
        {
        }
    }


}
