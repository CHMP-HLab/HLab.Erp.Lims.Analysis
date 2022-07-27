using System;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    public class ProductFormsListViewModel: Core.EntityLists.EntityListViewModel<Form>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";

        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => true;
        protected override bool CanExecuteDelete(Form form, Action<string> errorAction) => true;

        public ProductFormsListViewModel(Injector i) : base(i, c => c
            .Column("Name")
                .Header("{Name}")
                .Link(e => e.Name)
                .Filter()
                    .Link(e => e.Name)
            .Column("Icon")
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