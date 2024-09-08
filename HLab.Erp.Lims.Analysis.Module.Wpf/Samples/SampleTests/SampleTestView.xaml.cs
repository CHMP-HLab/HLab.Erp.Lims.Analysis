using System.Windows.Controls;
using HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests;

/// <summary>
/// Logique d'interaction pour SampleTestView.xaml
/// </summary>
public partial class SampleTestView : UserControl, IView<SampleTestViewModel>, IViewClassDocument
{
    public SampleTestView()
    {
        InitializeComponent();
    }
}