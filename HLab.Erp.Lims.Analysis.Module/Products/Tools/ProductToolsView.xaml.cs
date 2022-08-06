using System.Windows.Controls;
using HLab.Erp.Acl.Users;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Products.Tools;

/// <summary>
/// Logique d'interaction pour ImportUsers.xaml
/// </summary>
public partial class ProductToolsView : UserControl, IView<ProductToolsViewModel>, IViewClassDocument
{
    public ProductToolsView()
    {
        InitializeComponent();
    }
}