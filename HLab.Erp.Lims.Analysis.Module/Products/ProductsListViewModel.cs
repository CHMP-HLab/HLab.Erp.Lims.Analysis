using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
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

        protected override bool CanExecuteAdd() => Erp.Acl.IsGranted(AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteDelete() => Erp.Acl.IsGranted(AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteImport() => Erp.Acl.IsGranted(AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteExport() => true;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}