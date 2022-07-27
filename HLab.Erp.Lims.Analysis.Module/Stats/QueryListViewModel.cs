
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    public class QueryListViewModel : Core.EntityLists.EntityListViewModel<StatQuery>
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "tools";
        }

        public QueryListViewModel(Injector i) : base(i, c => c
            .Column("Name")
                .Header("{Name}")
                .Width(500)
                .Link(s => s.Nom)
                    .Filter()
        )
        {
        }
    }
}
