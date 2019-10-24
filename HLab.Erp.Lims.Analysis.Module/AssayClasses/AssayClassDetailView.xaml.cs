using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.AssayClasses
{
    /// <summary>
    /// Logique d'interaction pour AssayView.xaml
    /// </summary>
    public partial class AssayClassDetailView : UserControl, IView<AssayClassViewModel>
    {
        public AssayClassDetailView()
        {
            InitializeComponent();
        }
    }
}
