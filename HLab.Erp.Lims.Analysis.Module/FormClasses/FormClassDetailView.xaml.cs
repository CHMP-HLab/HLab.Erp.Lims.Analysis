using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class FormClassDetailView : UserControl, IView<FormClassViewModel>, IViewClassDetail
    {
        public FormClassDetailView()
        {
            InitializeComponent();
        }
    }
}
