using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    class QueryListViewModel : EntityListViewModel<Requete>
    {
        private readonly IErpServices _erp;
        [Import] public QueryListViewModel(IErpServices erp)
        {
            AddAllowed = false;
            DeleteAllowed = false;

            _erp = erp;
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
