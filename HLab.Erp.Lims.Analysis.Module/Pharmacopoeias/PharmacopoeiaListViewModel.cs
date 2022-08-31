
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Lims.Analysis.Data.Entities;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias;

public class PharmacopoeiasListViewModel: Core.EntityLists.EntityListViewModel<Pharmacopoeia>, IMvvmContextProvider
{
    public class Bootloader : ParamBootloader { }

    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(Pharmacopoeia pharmacopoeia, Action<string> errorAction) => true;

    public PharmacopoeiasListViewModel(Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}")
        .Width(250)
        .Localize(e => e.Name)
        .OrderByAsc(0)
        .Icon(p => p.IconPath)
        .Link(e => e.Name)
        .Filter()

        .Column("Abbreviation")
        .Header("{Abbreviation}")
        .Width(250)
        .Link(e => e.Abbreviation)
        .Filter()
    )
    {

    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

}