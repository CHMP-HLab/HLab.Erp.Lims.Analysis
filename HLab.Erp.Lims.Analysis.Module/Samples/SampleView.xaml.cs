using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using HLab.Base;
using HLab.Base.Wpf;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Views;
using Microsoft.Xaml.Behaviors.Layout;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    /// <summary>
    /// Logique d'interaction pour SampleView.xaml
    /// </summary>
    public partial class SampleView : UserControl , IView<SampleViewModel>, IViewClassDocument
    {
        public SampleView()
        {
            InitializeComponent();

            this.SetHighlights(vm=>vm.Workflow);
        }
    }
}
