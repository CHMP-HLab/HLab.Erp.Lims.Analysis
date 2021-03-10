using System.Windows;
using System.Windows.Controls;
using HLab.Base.Wpf;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    using H = DependencyHelper<ColumnDescriptionBlock>;


    /// <summary>
    /// Logique d'interaction pour TestView.xaml
    /// </summary>
    public partial class ColumnDescriptionBlock : UserControl
    {
        public ColumnDescriptionBlock()
        {
            InitializeComponent();
        }

        public static DependencyProperty TitleProperty = H.Property<string>().BindsTwoWayByDefault.Register();
        public static DependencyProperty DescriptionProperty = H.Property<string>().OnChange(
            (e, a) =>
            {
                var v = a.NewValue?.TrimEnd('\r', '\n', ' ');
                if (v != e.Description) e.Description = v;
            }
            ).BindsTwoWayByDefault.Register();
        public static DependencyProperty IconPathProperty = H.Property<string>().BindsTwoWayByDefault.Register();

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public string IconPath
        {
            get => (string) GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }
    }
}
