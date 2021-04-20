using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    public class PharmacopoeiaListViewModel: EntityListViewModel<Pharmacopoeia>, IMvvmContextProvider
    {
        protected override void Configure()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column
                    .Header("{Name}").Localize()
                    .Width(250)
                    .Content(e => e.Name)
                .Icon(p => p.IconPath)
                .Column
                    .Header("{Abbreviation}")
                    .Width(250)
                    .Content(e => e.Abbreviation)
                );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>(f => f.Title("{Name}")
                    .Link(List, e => e.Name));
            }
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}
