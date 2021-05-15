using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers
{
    public class ManufacturersListViewModel : EntityListViewModel<Manufacturer>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        { }

        public ManufacturersListViewModel() : base(c => c
//                .AddAllowed()
// TODO                .DeleteAllowed()
            .Column()
            .Header("{Name}")
            .Width(250).Link(e => e.Name)
                .Filter()
                    .Header("{Name}")

            .Column()
                .Icon(m => m.Country?.IconPath ?? "")
                .OrderBy(s => s.Country.Name)
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}
