using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    public class PharmacopoeiaListViewModel: EntityListViewModel<Pharmacopoeia>, IMvvmContextProvider
    {
        private readonly IErpServices _erp;

        [Import]
        public PharmacopoeiaListViewModel(IErpServices erp)
        {
            AddAllowed = true;
            DeleteAllowed = true;

            _erp = erp;
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
