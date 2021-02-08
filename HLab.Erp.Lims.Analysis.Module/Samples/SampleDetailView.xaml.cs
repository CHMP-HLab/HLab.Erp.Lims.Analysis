using HLab.Erp.Core;
using HLab.Mvvm.Annotations;
using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Base.Wpf;

namespace HLab.Erp.Lims.Analysis.Module
{
    /// <summary>
    /// Logique d'interaction pour SampleDetailView.xaml
    /// </summary>
    public partial class SampleDetailView : UserControl , IView<SampleViewModel>, IViewClassDetail

    {
        public SampleDetailView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
