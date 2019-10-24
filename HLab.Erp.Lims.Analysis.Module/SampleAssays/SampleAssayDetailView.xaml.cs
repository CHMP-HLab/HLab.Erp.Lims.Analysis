using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleAssays
{
    /// <summary>
    /// Logique d'interaction pour AssayView.xaml
    /// </summary>
    public partial class SampleAssayDetailView : UserControl, IView<SampleAssayViewModel>
    {
        public SampleAssayDetailView()
        {
            InitializeComponent();
        }
    }
}
