﻿using System;
using System.Windows;
using HLab.Base;
using HLab.Core;
using HLab.DependencyInjection;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Core.ApplicationServices;
using HLab.Erp.Lims.Monographs.Loader;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Wpf;
using HLab.Notify;
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
            ///MessageBox.Show("Startup");

            base.OnStartup(e);

            var container = new DependencyInjectionContainer();
            container.ExportInitialize<OptionsServices>((c, a, o) => o.OptionsPath = "HLab.Erp");
            container.Configure(c => c.Export<EventHandlerServiceWpf>().As<IEventHandlerService>());

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
            
#if !DEBUG
            try
            {
#endif
                boot.Configure();

                var doc = container.Locate<IDocumentService>();
                doc.MainViewModel = container.Locate<MainWpfViewModel>();


                boot.Boot();
#if !DEBUG
            }
            catch(Exception ex)
            {
                var view = new ExceptionView {Exception = ex};
                view.ShowDialog();
            }
#endif
        }
    }
}
