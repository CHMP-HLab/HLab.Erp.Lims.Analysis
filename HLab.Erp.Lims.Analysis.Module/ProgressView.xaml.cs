using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module
{
    /// <summary>
    /// Logique d'interaction pour ProgressView.xaml
    /// </summary>
    public partial class ProgressView : UserControl, IView<ViewModeDefault, ProgressViewModel>
    {
        public ProgressView()
        {
            InitializeComponent();
        }
    }
}
