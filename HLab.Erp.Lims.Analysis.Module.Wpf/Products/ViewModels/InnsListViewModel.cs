using System;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

public class InnsListViewModel: Core.EntityLists.EntityListViewModel<Inn>, IMvvmContextProvider
{
    public class Bootloader : ParamBootloader { }

    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(Inn inn, Action<string> errorAction) => true;

    public InnsListViewModel(Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}")
        .IconPath("Icons/Entities/Inn")
        .Width(250)
        .Localize(e => e.Name)
        .Icon(p => p.IconPath)
        .Link(e => e.Name)
        .OrderByAsc(0)
        .Filter()

    )
    {

    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

}