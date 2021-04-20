using System.Windows.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    using H = DependencyHelper<TestView>;


    public interface IDescriptionViewClass : IViewClass {}

    class SampleTestInlineViewModel : ViewModel<SampleTest>
    {

    }

    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class TestView : UserControl, IView<SampleTestInlineViewModel> , IDescriptionViewClass
    {
        public TestView()
        {
            InitializeComponent();
        }

    }
}
