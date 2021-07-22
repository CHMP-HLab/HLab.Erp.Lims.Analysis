
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Lims.Analysis.Data.Entities;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductFormsListViewModel: EntityListViewModel<Form>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";

        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Form form, Action<string> errorAction) => true;

        public ProductFormsListViewModel() : base(c => c
            .Column()
                .Header("{Name}")
                .Link(e => e.Name)
                .Filter()
                    .Link(e => e.Name)
            .Column()
                .Header("{Icon}")
                .Icon((s) => s.IconPath)
                .OrderBy(s => s.Name)            
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}