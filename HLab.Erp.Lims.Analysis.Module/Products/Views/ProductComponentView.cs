using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Erp.Lims.Analysis.Module.Products.ViewModels;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Products.Views
{
    /// <summary>
    /// Logique d'interaction pour ProductView.xaml
    /// </summary>
    public partial class ProductComponentView : UserControl, IView<ProductComponentViewModel>, IViewClassDetail, IViewClassDocument
    {
        public ProductComponentView()
        {
            InitializeComponent();
        }
    }
}
