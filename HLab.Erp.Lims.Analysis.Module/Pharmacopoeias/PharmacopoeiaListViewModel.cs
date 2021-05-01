using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    public class PharmacopoeiasListViewModel: EntityListViewModel<Pharmacopoeia>, IMvvmContextProvider
    {
        public PharmacopoeiasListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()
                .Column()
                    .Header("{Name}").Localize()
                    .Width(250)
                    .Content(e => e.Name)
                    .Localize()
                    .Icon(p => p.IconPath)
                    .Link(e => e.Name)
                        .Filter()
                .Column()
                    .Header("{Abbreviation}").Localize()
                    .Width(250)
                    .Link(e => e.Abbreviation)
                        .Filter()
        )
        {

        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}
