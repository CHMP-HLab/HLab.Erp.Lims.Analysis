using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Network;

namespace HLab.Erp.Data.Wpf
{
    /// <summary>
    /// Logique d'interaction pour DatabaseConfig.xaml
    /// </summary>
    public partial class DatabaseConfigView : UserControl, IView<ViewModeDefault,ConnectionData>, IViewClassDefault
    {
         [Import] public IPScanner PostgresqlServers { get; }

        public DatabaseConfigView()
        {
            InitializeComponent();

            PostgresqlServers.Scan(5432);
        }
    }
}
