﻿using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

public class ProductsListPopupViewModel : Core.EntityLists.EntityListViewModel<Product>, IMvvmContextProvider
{
    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

    public ProductsListPopupViewModel(Injector i) : base(i, c => c
        .Header("{Products}")
        .Column("Reference")
        .Header("{Ref}")
        .Link(p => p.Caption)
        .OrderByAsc()
                
        .Column("Name")
        .Header("{Name}")
        .Link(p => p.Name)
        .Filter()
        .IconPath("Icons/Entities/Inn")

        .Column("Variant")
        .Header("{Dose}")
        .Link(p => p.Variant)
        .Filter()
        .IconPath("Icons/Entities/Products/Dose")
        .Link(p => p.Variant)

        .FormColumn( p => p.Form)

        //TODO check list it seams strange
    )
    {
    }
}