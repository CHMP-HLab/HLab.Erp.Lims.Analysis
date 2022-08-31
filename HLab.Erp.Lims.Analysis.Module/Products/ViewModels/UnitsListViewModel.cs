using System;
using HLab.Core.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

public class UnitsListViewModel: Core.EntityLists.EntityListViewModel<Unit>, IMvvmContextProvider
{
    public class Bootloader : ParamBootloader
    {
        public override void Load(IBootContext bootstrapper)
        {
            Menu.RegisterMenu("param/units", "{Units}", null, "Icons/Entities/Unit");
            base.Load(bootstrapper);
        }
        public override string MenuPath => "param/units";
    }

    //Todo : rights to configure units
    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(Unit inn, Action<string> errorAction) => true;

    public UnitsListViewModel(Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}").IconPath("Icons/Entities/Unit")
        .Width(250)
        .Localize(e => e.Name)
        .Icon(p => p.IconPath)
        .Link(e => e.Name)
        .Filter()
    )
    {

    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

}