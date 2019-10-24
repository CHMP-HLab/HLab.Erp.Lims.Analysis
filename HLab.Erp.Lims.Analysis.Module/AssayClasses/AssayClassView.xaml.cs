using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.AssayClasses
{
    /// <summary>
    /// Logique d'interaction pour AssayClassView.xaml
    /// </summary>
    public partial class AssayClassView : UserControl,IView<AssayClassViewModel>, IViewClassDocument
    {
        public AssayClassView()
        {
            InitializeComponent();
            DataContextChanged += AssayClassView_DataContextChanged;
        }

        private void AssayClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is AssayClassViewModel oldVm)
                oldVm.PropertyChanged -= Vm_PropertyChanged;
            if (e.NewValue is AssayClassViewModel vm)
            {
                XamlEditor.Text = vm.Xaml;
                CodeEditor.Text = vm.Cs;
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Xaml")
            //    XamlEditor.Text = ((AssayClassViewModel) DataContext).Xaml;
            //if (e.PropertyName == "Code")
            //    CodeEditor.Text = ((AssayClassViewModel) DataContext).Code;
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, XamlEditor))
                ((AssayClassViewModel) DataContext).Xaml = XamlEditor?.Text;
            if (ReferenceEquals(sender, CodeEditor))
                ((AssayClassViewModel) DataContext).Xaml = XamlEditor?.Text;
        }
    }
}
