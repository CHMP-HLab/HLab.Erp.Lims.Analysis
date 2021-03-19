using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    /// <summary>
    /// Logique d'interaction pour ProductView.xaml
    /// </summary>
    public partial class PharmacopoeiaView : UserControl, IView<PharmacopoeiaViewModel>, IViewClassDetail, IViewClassDocument
    {
        public PharmacopoeiaView()
        {
            InitializeComponent();
        }
    }
}
