using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Erp.Core;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    /// <summary>
    /// Logique d'interaction pour ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl, IView<ProductViewModel>, IViewClassDetail, IViewClassDocument
    {
        public ProductView()
        {
            InitializeComponent();
        }
    }
}
