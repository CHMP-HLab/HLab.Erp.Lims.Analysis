using System;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

public class FormClassesListViewModel : Core.EntityLists.EntityListViewModel<FormClass>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    readonly IAclService _acl;

    protected override bool CanExecuteAdd(Action<string> errorAction)
    {
        return _acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
    }

    protected override bool CanExecuteDelete(FormClass target,Action<string> errorAction)
    {
        RemoveErrorMessage("Delete");
        return _acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
    }

    public FormClassesListViewModel(IAclService acl, Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}")
        .Width(200)
        .Content(s => s.Name)
        .Icon( s => s.IconPath)

        .Column("Class")
        .Header("{Class}")
        .Width(100).Content(s => s.Class??"")
        .OrderBy(s => s.Class)
    )
    {
        _acl = acl;
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}