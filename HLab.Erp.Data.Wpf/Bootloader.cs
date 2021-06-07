using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf
{
    public class ErpDataBootloader : IBootloader
    {
        private readonly IDataService _data;
        private readonly IMvvmService _mvvm;

        public ErpDataBootloader(IDataService data, IMvvmService mvvm)
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

                var data = new ConnectionData();

                var view = _mvvm.MainContext.GetView(data, typeof(ViewModeDefault), typeof(IViewClassDefault));

                var dialog = new Window
                {
                    Content = view,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                if (dialog.ShowDialog() ?? false)
                {
                    return $"Host={data.Server};Username={data.UserName};Password={data.Password};Database={data.Database}";;

                }


                return "";
            });
        }
    }
}
