using System.Windows.Controls;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    class SampleTestInlineViewModel : ViewModel<SampleTest>
    {

    }

    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class TestView : UserControl, IView<SampleTestInlineViewModel> , IListElementViewClass
    {
        public TestView()
        {
            InitializeComponent();
        }

    }
}
