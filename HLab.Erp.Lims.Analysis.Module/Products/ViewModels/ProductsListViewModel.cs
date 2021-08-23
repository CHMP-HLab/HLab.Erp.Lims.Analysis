using System;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    public class ProductsListViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader { }
        public ProductsListViewModel() : base(c => c
               .Column()
               .Header("{Category}")
               .Width(100)
               .Content(e => e.Category == null ? "" : e.Category.Name)
               .OrderBy(e => e.Category?.Name)

               .Column()
               .Header("{Inn}")
               .IconPath("Icons/Entities/Products/Inn")
               .Width(300)
               .Link(e => e.Inn)
               .OrderBy(e => e.Inn)
                .Filter()

               .Column()
               .Header("{Dose}")
               .IconPath("Icons/Entities/Products/Dose")
               .Width(200)
               .Link(e => e.Dose)
               .OrderBy(e => e.Dose)
                    .Filter()

               .FormColumn(e => e.Form)
        )
        {
        }

        protected override bool CanExecuteAdd(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteDelete(Product product, Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteImport(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteExport(Action<string> errorAction) => true;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}