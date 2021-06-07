﻿using System;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;

using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public ProductsListPopupViewModel() : base(c => c
                .Header("{Products}")
                //.AddAllowed()
                .Column()
                    .Header("{Ref}")
                    .Link(p => p.Caption)
                
                .Column()
                    .Header("{Inn}")
                    .Link(p => p.Inn)
                    .Filter()
                    .IconPath("Icons/Entities/Products/Inn")

                .Column()
                    .Header("{Dose}")
                    .Link(p => p.Dose)
                    .Filter()
                    .IconPath("Icons/Entities/Products/Dose")
                    .Link(p => p.Dose)

                .FormColumn( p => p.Form)

        )
        {
        }
    }


}
