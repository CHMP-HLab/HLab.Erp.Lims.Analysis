using System;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using Grace.DependencyInjection.Attributes;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public ProductsListPopupViewModel() : base(c => c
            .Header("{Products}")
            .AddAllowed()
                .Column().Header("{Ref}").Content(p => p.Caption)
                .Column().Header("{Inn}").Content(p => p.Inn)
                    .Filter<TextFilter>()
                    .IconPath("Icons/Entities/Products/Inn")
                    .Link(p => p.Inn)
                .Column().Header("{Dose}").Content(p => p.Dose)
                    .Filter<TextFilter>()
                    .IconPath("Icons/Entities/Products/Dose")
                    .Link(p => p.Dose)
                .FormColumn( p => p.Form,p => p.FormId)

        )
        {
        }
    }


}
