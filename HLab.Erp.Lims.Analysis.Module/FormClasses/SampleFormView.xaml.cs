using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    /// <summary>
    /// Logique d'interaction pour SampleTestView.xaml
    /// </summary>
    public partial class SampleFormView : UserControl, IView<SampleFormViewModel>, IViewClassDocument
    {
        public SampleFormView()
        {
            InitializeComponent();
        }
    }
}
