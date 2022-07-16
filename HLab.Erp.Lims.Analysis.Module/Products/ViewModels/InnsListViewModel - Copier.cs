using System;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    public class UnitsListViewModel: EntityListViewModel<Unit>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        //Todo : rights to configure units
        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Unit inn, Action<string> errorAction) => true;

        public UnitsListViewModel() : base(c => c
            .Column("Name")
            .Header("{Name}").Localize().IconPath("Icons/Entities/Products/Unit")
            .Width(250).Content(e => e.Name)
                    .Localize()
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
}
