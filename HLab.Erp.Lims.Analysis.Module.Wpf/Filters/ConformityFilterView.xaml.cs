using System.Windows.Controls;
using HLab.Erp.Core.Wpf.ListFilters;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Filters;

/// <summary>
/// Logique d'interaction pour FilterEntityView.xaml
/// </summary>
public partial class ConformityFilterView : UserControl , IView<ViewModeDefault, IWorkflowFilter>, IFilterContentViewClass
{
    public ConformityFilterView()
    {
        InitializeComponent();
    }

    public void SetFocus()
    {
    }
}