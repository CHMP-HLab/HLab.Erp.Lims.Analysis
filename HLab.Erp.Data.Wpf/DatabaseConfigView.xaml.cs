using System.Windows;
using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Data.Wpf
{
    /// <summary>
    /// Logique d'interaction pour DatabaseConfig.xaml
    /// </summary>
    public partial class DatabaseConfigView : UserControl, IView<ConnectionDataViewModel>
    {

        public DatabaseConfigView()
        {
            InitializeComponent();
        }

        private void CbServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DialogResult = true;
            window?.Close();

        }
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DialogResult = false;
            window?.Close();

        }
    }
}
