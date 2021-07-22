using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    /// <summary>
    /// Logique d'interaction pour ProductView.xaml
    /// </summary>
    public partial class ProductCategoryView : UserControl
        , IView<ProductCategoryViewModel>
        , IViewClassDetail
        , IViewClassDocument
    {
        public ProductCategoryView()
        {
            InitializeComponent();
        }
    }
}
