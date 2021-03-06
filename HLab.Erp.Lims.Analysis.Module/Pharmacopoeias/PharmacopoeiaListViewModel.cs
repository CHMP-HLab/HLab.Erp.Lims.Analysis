﻿
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Lims.Analysis.Data.Entities;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    public class PharmacopoeiasListViewModel: EntityListViewModel<Pharmacopoeia>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Pharmacopoeia pharmacopoeia, Action<string> errorAction) => true;

        public PharmacopoeiasListViewModel() : base(c => c
            .Column()
            .Header("{Name}").Localize()
            .Width(250).Content(e => e.Name)
                    .Localize()
                    .Icon(p => p.IconPath)
                    .Link(e => e.Name)
                        .Filter()

                .Column()
                    .Header("{Abbreviation}").Localize()
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
}
