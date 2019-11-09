using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class SampleTestDetailView : UserControl, IView<SampleTestViewModel>
    {
        public SampleTestDetailView()
        {
            InitializeComponent();
        }
    }
}
