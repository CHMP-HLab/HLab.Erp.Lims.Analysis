using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class SampleFormDetailView : UserControl, IView<SampleFormViewModel>, IViewClassDetail, IViewClassDefault
    {
        public SampleFormDetailView()
        {
            InitializeComponent();
        }
    }
}
