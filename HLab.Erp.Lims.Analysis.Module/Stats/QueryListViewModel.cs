using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    class QueryListViewModel : EntityListViewModel<Requete>
    {
       protected override void Configure()
       {
            AddAllowed = false;
            DeleteAllowed = false;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                    .Column
                        .Header("{Name}")
                        .Width(500)
                        .Content(s => s.Nom)
                );

            using (List.Suspender.Get())
            {
            }

       }
    }
}
