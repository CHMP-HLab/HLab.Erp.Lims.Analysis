using System.Windows;
using HLab.Core;
using HLab.Erp.Core.Wpf;
using HLab.Notify.PropertyChanged;
using HLab.Notify.Wpf;

namespace LimsAnalysis.Loader
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var boot = new Bootloader();
            boot.Container.ExportInitialize<OptionsServicesWpf>((c, a, o) => o.SetRegistryPath("CHMP"));
            NotifyHelper.EventHandlerService = new EventHandlerServiceWpf(); // boot.Container.Locate<IEventHandlerService>();
            //boot.Container.ExportInitialize<BootLoaderErpWpf>((c, a, o) => o.SetMainViewMode(typeof(ViewModeKiosk)));

            boot.Boot();
        }
    }
}
