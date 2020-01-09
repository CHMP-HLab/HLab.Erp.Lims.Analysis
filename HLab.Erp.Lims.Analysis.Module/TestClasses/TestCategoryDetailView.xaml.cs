using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Erp.Core.Tools.Details;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class TestCategoryDetailView : UserControl, IView<TestCategoryViewModel>, IViewClassDetail, IViewClassDocument
    {
        public TestCategoryDetailView()
        {
            InitializeComponent();
        }
    }
}
