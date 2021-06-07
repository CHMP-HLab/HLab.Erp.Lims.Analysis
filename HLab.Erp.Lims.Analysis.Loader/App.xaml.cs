using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows;
using HLab.Bugs.Wpf;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.Core.DebugTools;
using HLab.Erp.Acl;
using HLab.Erp.Acl.AuditTrails;
using HLab.Erp.Acl.LoginServices;
using HLab.Erp.Base.Data;
using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Core;
using HLab.Erp.Core.DragDrops;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Localization;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Core.WebService;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.Manufacturers;
using HLab.Erp.Lims.Monographs.Data;
using HLab.Erp.Units;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;
using HLab.Ioc;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Application.Wpf;
using HLab.Mvvm.Flowchart;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using HLab.Notify.Wpf;
using HLab.Options;
using HLab.Options.Wpf;


namespace HLab.Erp.Lims.Analysis.Loader
{



    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private T Locate<T>() => Locator<T>.Locate();

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            
            try
            {
#endif

            base.OnStartup(e);

            var parser = new AssemblyParser();
            Locator.Configure();

            SingletonLocator<IOptionsService>.Set<OptionsServices>();

            SingletonLocator<IEventHandlerService>.Set<EventHandlerServiceWpf>();
            SingletonLocator<IAclService>.Set<AclService>();
            SingletonLocator<IAclHelper>.Set<AclHelperWpf>();
            SingletonLocator<IDebugLogger>.Set<DebugLogger>();
            SingletonLocator<IDataService>.Set<DataService>();
            SingletonLocator<IMessageBus>.Set<MessageBus>();
            SingletonLocator<IDragDropService>.Set<DragDropServiceWpf>();
            SingletonLocator<IDialogService>.Set<DialogService>();
            SingletonLocator<ICurrencyService>.Set<CurrencyService>();
            
            SingletonLocator<IUnitService>.Set<UnitService>();

            SingletonLocator<IGraphService>.Set<GraphService>();
            SingletonLocator<IBrowserService>.Set<BrowserViewModel>();
            SingletonLocator<IIconService>.Set<IconService>();
            SingletonLocator<ILocalizationService>.Set<LocalizationService>();
            SingletonLocator<IMvvmService>.Set<MvvmServiceWpf>();
            SingletonLocator<IErpServices>.Set<ErpServices>();
            SingletonLocator<IApplicationInfoService>.Set<ApplicationInfoService>();
            SingletonLocator<IDocumentService>.Set<DocumentServiceWpf>();
            SingletonLocator<IMenuService>.Set<MenuService>();

            SingletonLocator<LocalizeFromDb>.Set<LocalizeFromDb>();

            Locator<ILoginViewModel>.Set<LoginViewModel>();
            Locator<IAuditTrailProvider>.Set<AuditTrailMotivationViewModel>();
            Locator<ISelectedMessage>.Set<SelectedMessage>();

            SingletonLocator<MainWpfViewModel>.Set<MainWpfViewModel>();



            Locator.SetOpenGenericFactory(typeof(IEntityListHelper<>),typeof(EntityListHelper<>));
            Locator.SetOpenGenericFactory(typeof(IColumnsProvider<>),typeof(ColumnsProvider<>));
            Locator.SetOpenGenericFactory(typeof(IObservableQuery<>),typeof(ObservableQuery<>));
            Locator.SetOpenGenericFactory(typeof(IEntityListViewModel<>),typeof(EntityListViewModel<>));
            Locator.SetOpenGenericFactory(typeof(IDataLocker<>),typeof(DataLocker<>));

            NotifyHelper.EventHandlerService = Locate<IEventHandlerService>();




            //Locator.SetFactory(typeof(IDataLocker<>),);

/*            

            
            .Export(typeof(DataLocker<>)).As(typeof(IDataLocker<>))

              .Export(typeof(ColumnsProvider<>)).As(typeof(IColumnsProvider<>))
             .Export(typeof(ListableEntityListViewModel<>)).As(typeof(IListableEntityListViewModel<>))
             .Export(typeof(ColumnConfigurator<,,>)).As(typeof(IColumnConfigurator<,,>))
             .Export(typeof(ColumnHelper<>)).As(typeof(IColumn<>.IHelper))
             .Export(typeof(ObservableQuery<>)).As(typeof(ObservableQuery<>))
             .Export(typeof(EntityListHelper<>)).As(typeof(IEntityListHelper<>))

             .Export<MvvmContext>().As<IMvvmContext>()
             .Export<SelectedMessage>().As<ISelectedMessage>()
             .Export<DetailsPanelViewModel>().As<DetailsPanelViewModel>()
             
             );
;
                
*/
            //boot.Scope.ExportInitialize<BootLoaderErpWpf>(b => b.SetMainViewMode(typeof(ViewModeKiosk)));


            //var a0 = boot.LoadDll("HLab.Erp.Core.Wpf");
            var a01 = parser.LoadDll("HLab.Options.Wpf");
            var a3 = parser.LoadDll("HLab.Notify.Wpf");
            var a2 = parser.LoadDll("HLab.Erp.Base.Wpf");
            //  var b0 = boot.LoadDll("HLab.Mvvm");
            //  var c0 = boot.LoadDll("HLab.Mvvm.Wpf");
            //  var d0 = boot.LoadDll("HLab.Erp.Data");
            var d1 = parser.LoadDll("HLab.Erp.Data.Wpf");
            var e0 = parser.LoadDll("HLab.Erp.Acl.Wpf");
            var a1 = parser.LoadDll("HLab.Erp.Workflows.Wpf");
            var g0 = parser.LoadDll("HLab.Erp.Lims.Analysis.Data");
            var g2 = parser.LoadDll("HLab.Erp.Lims.Analysis.Module");
            //var g1 = boot.LoadDll("HLab.Erp.Lims.Monographs.Module");

            parser.LoadModules();

            parser.Add<IView>(t => EnumerableLocator<IView>.AddAutoFactory(t));
            parser.Add<IViewModel>(t => EnumerableLocator<IViewModel>.AddAutoFactory(t));

            parser.Add<IBootloader>(t => EnumerableLocator<IBootloader>.AddAutoFactory(t));
            parser.Add<IToolGraphBlock>(t => EnumerableLocator<IToolGraphBlock>.AddAutoFactory(t));
            parser.Add<IEntityListViewModel>(t =>
            {
                if(t.IsGenericType) return;
                foreach(var i in t.GetInterfaces().Where(i => i.IsGenericType))
                {
                    if(i.GetGenericTypeDefinition()==typeof(IEntityListViewModel<>))
                    {
                        var type = i.GetGenericArguments()[0];
                        Locator.Set(i,t);
                    }
                }
            });
            parser.Parse();

            Locator.InitSingletons();

            var options = Locate<IOptionsService>();
                options.OptionsPath = "HLab.Erp";
                options.AddProvider(new OptionsProviderRegistry());

            //STATIC IMPORTS//
            NotifyHelper.EventHandlerService = Locate<IEventHandlerService>();
            WorkflowAnalysisExtension.Acl = Locate<IAclService>();

            var mvvm = Locate<IMvvmService>();

            mvvm.Register(typeof(Customer), typeof(CustomerViewModel), typeof(IViewClassDocument), typeof(ViewModeDefault));
            mvvm.Register(typeof(Manufacturer), typeof(ManufacturerViewModel), typeof(IViewClassDocument), typeof(ViewModeDefault));
            mvvm.Register();

            var doc = Locate<IDocumentService>();
            doc.MainViewModel = Locate<MainWpfViewModel>();

            var boot = new Bootstrapper(()=> Locate<IEnumerable<IBootloader>>());

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
