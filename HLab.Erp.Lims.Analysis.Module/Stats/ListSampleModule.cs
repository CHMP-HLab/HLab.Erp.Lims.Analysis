using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Stats
{
    using H = H<ListQueryModule>;

    public class ListQueryModule : NotifierBase, IBootloader
    {
        
        private readonly IErpServices _erp;

        public ListQueryModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }


        public ICommand OpenListQueryCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(QueryListViewModel))
        ));


        public void Load(IBootContext b)
        {
                _erp.Menu.RegisterMenu("tools/queries", "{Queries}",
                    OpenListQueryCommand,
                    "Icons/Entities/Sample");
        }
    }
}