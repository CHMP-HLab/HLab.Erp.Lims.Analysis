using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers
{
    public class ManufacturersListViewModel: EntityListViewModel<Manufacturer>, IMvvmContextProvider
    {
        private readonly IErpServices _erp;

        [Import]
        public ManufacturersListViewModel(IErpServices erp)
        {
            AddAllowed = true;
            DeleteAllowed = true;

            _erp = erp;
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                .Column
                    .Header("{Name}")
                    .Width(250)
                    .Content(e => e.Name)
                .Column
                    .Icon(m => m.Country?.IconPath ?? "")
                    .OrderBy(s => s.Country.Name)
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
