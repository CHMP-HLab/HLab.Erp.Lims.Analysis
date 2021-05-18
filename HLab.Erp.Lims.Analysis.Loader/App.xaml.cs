using System;
using System.Runtime.ExceptionServices;
using System.Windows;
using Grace.DependencyInjection;
using HLab.Bugs.Wpf;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
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

            var boot = new Bootstrapper();

            //Dependency injection configuration
            boot.Scope.Configure(c => c
                .Export(typeof(ListableEntityListViewModel<>))
                .As(typeof(IListableEntityListViewModel<>))
                .ProcessAttributes()
                );

            boot.Scope.Configure(c => c.ExportInitialize<OptionsServices>(s => s.OptionsPath = "HLab.Erp"));

            boot.Scope.Configure(c => c.Export<EventHandlerServiceWpf>().As<IEventHandlerService>());

            boot.Scope.Configure(c => c.Export(typeof(DataLocker<>)).As(typeof(IDataLocker<>)));
            boot.Scope.Configure(c => c.Export(typeof(EntityListHelper<>)).As(typeof(IEntityListHelper<>)));

            //boot.Scope.ExportInitialize<BootLoaderErpWpf>(b => b.SetMainViewMode(typeof(ViewModeKiosk)));


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
            var g0 = boot.LoadDll("HLab.Erp.Lims.Analysis.Data");
            var g2 = boot.LoadDll("HLab.Erp.Lims.Analysis.Module");
            //var g1 = boot.LoadDll("HLab.Erp.Lims.Monographs.Module");



            boot.LoadModules();
            boot.Export<IEntityListViewModel>(typeof(IEntityListViewModel<>));

            boot.Export<IView>();
            boot.Export<IViewModel>();



            //STATIC IMPORTS//
            NotifyHelper.EventHandlerService = boot.Scope.Locate<IEventHandlerService>();
            WorkflowAnalysisExtension.Acl = boot.Scope.Locate<IAclService>();

            var mvvm = boot.Scope.Locate<IMvvmService>();

            mvvm.Register(typeof(Customer), typeof(CustomerViewModel), typeof(IViewClassDocument), typeof(ViewModeDefault));
            mvvm.Register(typeof(Manufacturer), typeof(ManufacturerViewModel), typeof(IViewClassDocument), typeof(ViewModeDefault));
            mvvm.Register();

            var doc = boot.Scope.Locate<IDocumentService>();
            doc.MainViewModel = boot.Scope.Locate<MainWpfViewModel>();

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
