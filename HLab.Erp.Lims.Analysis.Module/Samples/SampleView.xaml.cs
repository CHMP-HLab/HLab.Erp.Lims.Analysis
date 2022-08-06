using System.Windows.Controls;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.Samples;

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