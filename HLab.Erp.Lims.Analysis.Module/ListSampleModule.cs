using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Lims.Analysis.Module.ListSampleModule>;

namespace HLab.Erp.Lims.Analysis.Module
{
    public class ListSampleModule : IPostBootloader
    {
        
        private readonly IErpServices _erp;

        [Import] public ListSampleModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }

        public ICommand OpenListSampleCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListSampleViewModel))
        ));

        public void Load()
        {
            _erp.Menu.RegisterMenu("data", "samples", "Samples",
                OpenListSampleCommand,
                _erp.Icon.GetIcon("icons/Sample"));
        }
    }
}