using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    using H = H<QueryViewModel>;

    public class QueryViewModel :ViewModel<Requete>
    {
        QueryViewModel() => H.Initialize(this);
    }
}
