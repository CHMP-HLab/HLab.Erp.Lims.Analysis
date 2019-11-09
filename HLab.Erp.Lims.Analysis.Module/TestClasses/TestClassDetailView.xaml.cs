using System.Windows.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class TestClassDetailView : UserControl, IView<TestClassViewModel>
    {
        public TestClassDetailView()
        {
            InitializeComponent();
        }
    }
}
