using System.Windows;
using System.Windows.Controls;

using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module
{
    using H = DependencyHelper<ProgressView>;
    /// <summary>
    /// Logique d'interaction pour ProgressView.xaml
    /// </summary>
    public partial class ProgressView : UserControl//, IView<ViewModeDefault, ProgressViewModel>
    {
        public ProgressView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            H.Property<double>()
            .OnChange((e, a) =>
            {
                e.ProgressBar.Value = a.NewValue;
                e.Label.Content = a.NewValue;
            })
            .Register();

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

    }
}
