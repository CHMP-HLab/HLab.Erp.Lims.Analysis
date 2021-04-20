using System;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public static class ListsExtensions
    {
        public static IColumnConfigurator<T> FormColumn<T>(this IColumnConfigurator<T> c, Func<T, Form> getForm)
            => c
                .Column
                    .Header("{Form}")
                    .Width(150)
                    .Content(e => getForm(e)?.Name)
                    .Localize()
                    .Icon(e => getForm(e)?.IconPath ?? "");

    }


    public class ProductsListPopupViewModel : EntityListViewModel<Product>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Products}";
        protected override void Configure()
        {
            AddAllowed = true;

            Columns.Configure(c => c
                .Column.Header("{Ref}").Content(p => p.Caption)
                .Column.Header("{Inn}").Content(p => p.Inn)
                .Column.Header("{Dose}").Content(p => p.Dose)
                .FormColumn(p => p.Form)
            );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>()
                    .Title("{Inn}")
                    .IconPath("Icons/Entities/Products/Inn")
                    .Link(List,p => p.Inn);

                Filter<TextFilter>()
                    .Title("{Dose}")
                    .IconPath("Icons/Entities/Products/Dose")
                    .Link(List,p => p.Dose);

                Filter<EntityFilter<Form>>()
                    .Title("{Form}")
                    .IconPath("Icons/Entities/Form")
                    .Link(List,p => p.FormId??-1);
            }

        }
    }


}
