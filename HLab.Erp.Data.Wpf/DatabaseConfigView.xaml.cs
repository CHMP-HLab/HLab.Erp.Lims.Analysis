using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
