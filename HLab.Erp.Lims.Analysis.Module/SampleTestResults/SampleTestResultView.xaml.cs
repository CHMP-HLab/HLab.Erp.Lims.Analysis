using System.Windows.Controls;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    /// <summary>
    /// Logique d'interaction pour SampleTestView.xaml
    /// </summary>
    public partial class SampleTestResultView : UserControl, IView<SampleTestResultViewModel>, IViewClassDocument
    {
        public SampleTestResultView()
        {
            InitializeComponent();
        }
    }
}
