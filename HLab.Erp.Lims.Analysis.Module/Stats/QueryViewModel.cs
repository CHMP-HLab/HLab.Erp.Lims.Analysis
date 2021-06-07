using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    using H = H<QueryViewModel>;

    public class QueryViewModel :ViewModel<Requete>
    {
        public QueryViewModel() => H.Initialize(this);
    }
}
