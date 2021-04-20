using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class FormsListViewModel: EntityListViewModel<Form>, IMvvmContextProvider
    {
        protected override void Configure()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(e => e.Name)
                .Column.Header("{Icon}").Icon((s) => s.IconPath).OrderBy(s => s.Name)
            );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f.Title("{Name}"))
                    .Link(List,e => e.Name);
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}