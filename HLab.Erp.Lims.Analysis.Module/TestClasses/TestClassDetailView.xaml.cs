using System.Windows.Controls;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses;

/// <summary>
/// Logique d'interaction pour TestView.xaml
/// </summary>
public partial class TestClassDetailView : UserControl, IView<TestClassViewModel>, IViewClassDetail
{
    public TestClassDetailView()
    {
        InitializeComponent();
    }
}