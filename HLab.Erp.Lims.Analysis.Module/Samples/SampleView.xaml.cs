using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using HLab.Base;
using HLab.Erp.Core;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Extensions;
using HLab.Mvvm.Lang;
using HLab.Mvvm.Views;

namespace HLab.Erp.Lims.Analysis.Module
{
    /// <summary>
    /// Logique d'interaction pour SampleView.xaml
    /// </summary>
    public partial class SampleView : UserControl , IView<SampleViewModel>, IViewClassDocument
    {
        public SampleView()
        {
            InitializeComponent();

            DataContextChanged += SampleView_DataContextChanged;
        }

        private void SampleView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if(this.TryGetViewModel(out var vm))
            {
                if (vm.Workflow.Highlights is INotifyCollectionChanged c)
                {
                    c.CollectionChanged += (target, arg) =>
                    {
                        switch (arg.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                foreach (var item in arg.NewItems)
                                {
                                    if(item is string s)
                                        Highlight(this,s);
                                }
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                Highlight(this,null);
                                break;
                        }
                    };
                }
            }
        }

        private void Highlight(object e, string name)
        {
            if (e == null) return;

            switch (e)
            {
                case IMandatoryNotFilled fw:
                    Highlight(fw,name);
                    break;
                case TextBox tb:
                    Highlight(tb,name);
                    break;

                case Panel p:
                    foreach (var c in p.Children)
                    {
                        Highlight(c,name);
                    }
                    break;

                case ContentControl contentControl:
                    Highlight(contentControl.Content,name);
                    break;
                case Popup popup:
                    Highlight(popup.Child,name);
                    break;
                case ComboBox comboBox:
                    Highlight(comboBox,name);
                    break;

                case ListView listView:
                case TextBlock localize:
                case Shape shape:
                case Decorator decorator:
                case Calendar calendar:
                    break;
                default:
                    break;

            }
        }

        private void Highlight(TextBox tb, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                tb.BorderThickness = new Thickness(0);
                tb.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            var binding = BindingOperations.GetBinding(tb, TextBox.TextProperty);
            if (binding != null)
            {
                var bName = binding.Path.Path.Split('.').Last();
                if (bName == name)
                {
                    tb.BorderThickness = new Thickness(1);
                    tb.BorderBrush = new SolidColorBrush(Colors.Red);
                }
            }
        }
        private void Highlight(ComboBox cb, string name)
        {
            //var binding = BindingOperations.GetBinding(cb, ComboBox.SelectedItemProperty);
            //if (binding != null)
            //{
            //    var bName = binding.Path.Path.Split('.').Last();
            //    if(bName == name)
            //        cb.Background = new SolidColorBrush(Colors.Red);
            //}
        }
        private void Highlight(IMandatoryNotFilled mnf, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                mnf.MandatoryNotFilled = false;

            else if (mnf is DependencyObject d)
            {
                var binding = BindingOperations.GetBinding(d, mnf.MandatoryProperty);
                if (binding != null)
                {
                    var bName = binding.Path.Path.Split('.').Last();
                    if(bName == name)
                        mnf.MandatoryNotFilled = true;
                }
            }
        }

    }
}
