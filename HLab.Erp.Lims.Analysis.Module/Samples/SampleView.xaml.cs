using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module
{
    /// <summary>
    /// Logique d'interaction pour SampleView.xaml
    /// </summary>
    public partial class SampleView : UserControl , IView<SampleViewModel>, IViewClassDocument
    {
        public SampleView()
        {
            InitializeComponent();
        }
    }
}
