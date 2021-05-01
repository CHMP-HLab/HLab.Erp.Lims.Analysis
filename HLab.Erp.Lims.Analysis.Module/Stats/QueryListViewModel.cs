using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    class QueryListViewModel : EntityListViewModel<Requete>
    {
       public QueryListViewModel() : base(c => c
                    .Column()
                        .Header("{Name}")
                        .Width(500)
                        .Content(s => s.Nom)
       
       )
       {
       }
    }
}
