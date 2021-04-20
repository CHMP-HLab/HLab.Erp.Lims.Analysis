using System;
using System.Windows;
using Grace.DependencyInjection;
using HLab.Core;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Manufacturers;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Wpf;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using HLab.Notify.Wpf;
using HLab.Options;

namespace HLab.Erp.Lims.Analysis.Loader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            
            try
            {
#endif

                base.OnStartup(e);

                var container = new DependencyInjectionContainer();
                
                // TODO :
                //container.Configure(e => e.ExportFactory<Func<OptionsServices>>(()=>new OptionsServices()));
                //container.ExportInitialize<OptionsServices>((c, a, o) => o.OptionsPath = "HLab.Erp")

                container.Configure(c => c.ExportInitialize<OptionsServices>(s => s.OptionsPath="HLab.Erp"));

                container.Configure(c => c.Export<EventHandlerServiceWpf>().As<IEventHandlerService>());

                container.Configure(c => c.Export(typeof(IDataLocker<>)).As(typeof(DataLocker<>)));

                //boot.Container.ExportInitialize<BootLoaderErpWpf>((c, a, o) => o.SetMainViewMode(typeof(ViewModeKiosk)));

                NotifyHelper.EventHandlerService =container.Locate<IEventHandlerService>();
                // new EventHandlerServiceWpf(); boot.


                var boot = container.Locate<Bootstrapper>();


                //var a0 = boot.LoadDll("HLab.Erp.Core.Wpf");
                var a01 = boot.LoadDll("HLab.Options.Wpf");
                var a3 = boot.LoadDll("HLab.Notify.Wpf");
                var a2 = boot.LoadDll("HLab.Erp.Base.Wpf");
                //  var b0 = boot.LoadDll("HLab.Mvvm");
                //  var c0 = boot.LoadDll("HLab.Mvvm.Wpf");
                //  var d0 = boot.LoadDll("HLab.Erp.Data");
                var d1 = boot.LoadDll("HLab.Erp.Data.Wpf");
                var e0 = boot.LoadDll("HLab.Erp.Acl.Wpf");
                var a1 = boot.LoadDll("HLab.Erp.Workflows.Wpf");
                var g0 = boot.LoadDll("HLab.Erp.Lims.Analysis.Module");
                //var g1 = boot.LoadDll("HLab.Erp.Lims.Monographs.Module");
            
                boot.Configure(container);

                var mvvm = container.Locate<IMvvmService>();

                mvvm.Register(typeof(Customer),typeof(CustomerViewModel),typeof(IViewClassDocument),typeof(ViewModeDefault));
                mvvm.Register(typeof(Manufacturer),typeof(ManufacturerViewModel),typeof(IViewClassDocument),typeof(ViewModeDefault));
                mvvm.Register();

                var doc = container.Locate<IDocumentService>();
                doc.MainViewModel = container.Locate<MainWpfViewModel>();


                boot.Boot();
#if !DEBUG
            }
            catch(Exception ex)
            {
                var view = new ExceptionView {Exception = ex};
                view.ShowDialog();
                throw;
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
#endif        
        }
    }
}
