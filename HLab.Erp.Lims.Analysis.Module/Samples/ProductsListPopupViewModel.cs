using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Products}";
        [Import]
        public ProductsListPopupViewModel()
        {

            Columns.Configure(c => c
                .Column
                .Header("{Inn}")
                .Width(200)
                .Content(p => p.Inn)
                .Column
                .Header("{Dose}")
                .Width(100)
                .Content(p => p.Dose)
                .FormColumn(p => p.Form)
            );
//                .Column("{Ref}", s => s.Caption)
            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f.Title("{Inn}")
                    .Link(List,e => e.Inn));

                Filter<TextFilter>(f => f.Title("{Dose}")
                    .Link(List,e => e.Dose));

                Filter<EntityFilter<Form>>()
                    .Link(List, e => e.FormId??-1);
            }

        }
    }
}