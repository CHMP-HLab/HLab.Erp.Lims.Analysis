using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using HLab.Base;
using HLab.Erp.Core;
using HLab.Erp.Core.EntitySelectors;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Extensions;
using HLab.Mvvm.Lang;
using HLab.Mvvm.Views;
using Microsoft.Xaml.Behaviors.Layout;

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
            if (!this.TryGetViewModel(out var vm)) return;
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

        private void Highlight(object e, string name)
        {
            if (e == null) return;

            switch (e)
            {
                case IMandatoryNotFilled fw:
                    HighlightUI(fw as UIElement, name);
                    break;

                case TextBox tb:
                    HighlightUI(tb,name);
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
                    HighlightUI(popup.Child,name);
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

        private static void RemoveHighlightUI(UIElement ct)
        {
            var al = AdornerLayer.GetAdornerLayer(ct);
            var ads = al?.GetAdorners(ct);
            if (ads == null) return;
            foreach (var ad in ads)
            {
                if (!(ad is AdornerContainer ac)) continue;
                if (ac.Child is MandatoryAdorner) al.Remove(ac);
            }
        }

        private static void HighlightUI(UIElement ui, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                RemoveHighlightUI(ui);
                return;
            }

            var binding = BindingOperations.GetBinding(ui, BindingProperty(ui));
            if (binding == null) return;

            var bName = binding.Path.Path.Split('.').Last();
            if (bName != name) return;

            var al = AdornerLayer.GetAdornerLayer(ui);
            var c = new AdornerContainer(ui)
            {
                IsHitTestVisible = false,
                Child = new MandatoryAdorner()
            };
            al?.Add(c);
        }

        private static DependencyProperty BindingProperty(UIElement ui)
        {
            return ui switch
            {
                NumTextBox ntb => NumTextBox.ValueProperty,
                TextBox tb => TextBox.TextProperty,
                IMandatoryNotFilled mnf => mnf.MandatoryProperty,
                Selector cb => Selector.SelectedValueProperty,
                _ => null
            };
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
