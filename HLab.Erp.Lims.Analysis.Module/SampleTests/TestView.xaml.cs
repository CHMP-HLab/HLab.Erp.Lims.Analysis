using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Base.Wpf;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using Outils;

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
