
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    public class QueryListViewModel : EntityListViewModel<Requete>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "tools";
        }

        public QueryListViewModel() : base(c => c
            .Column()
                .Header("{Name}")
                .Width(500)
                .Link(s => s.Nom)
                    .Filter()
        )
        {
        }
    }
}
