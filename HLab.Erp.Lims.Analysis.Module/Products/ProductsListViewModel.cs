using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ProductsListViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        protected override void Configure()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column
                    .Header("{Category}")
                    .Width(100)
                    .Content(e => e.Category?.Name)
                    .OrderBy(e => e.Category.Name)
                .Column
                    .Header("{Inn}")
                    .Width(300)
                    .Content(e => e.Inn)
                .Column
                    .Header("{Dose}")
                    .Width(200)
                    .Content(e => e.Dose)
                .FormColumn(e => e.Form)//.Header("{Form}").Content(e => e.Form.Name).Localize().Icon((s) => s.Form?.IconPath??"")
            );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f. Title("{Inn}"))
                    .IconPath("Icons/Entities/Products/Inn")
                    .Link(List,e => e.Inn);

                Filter<TextFilter>(f => f.Title("{Dose}"))
                    .IconPath("Icons/Entities/Products/Dose")
                    .Link(List,e => e.Dose);

                Filter<EntityFilter<Form>>(f => f.Title("{Form}"))
                    .Link(List,e => e.FormId??-1);
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}