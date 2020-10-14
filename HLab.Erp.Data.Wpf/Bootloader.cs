using System.Windows;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf
{

    public class ConnectionData
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Bootloader : IBootloader
    {
        private readonly IDataService _data;
        private readonly IMvvmService _mvvm;

        [Import] public Bootloader(IDataService data, IMvvmService mvvm)
        {
            _data = data;
            _mvvm = mvvm;
        }

        public void Load(IBootContext bootstrapper)
        {
            //if (_mvvm.ServiceState != ServiceState.Available)
            //{
            //    bootstrapper.Requeue();
            //    return;
            //}


            _data.SetConfigureAction(() =>
            {
                var connectionString = "";

                var data = new ConnectionData();

                var view = _mvvm.MainContext.GetView(data,typeof(ViewModeDefault),typeof(IViewClassDefault));

                var dialog = new Window{Content = view};

                if (dialog.ShowDialog()??false)
                {

                }

                return connectionString;
            });
        }
    }
}
