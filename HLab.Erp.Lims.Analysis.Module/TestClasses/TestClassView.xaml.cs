using System;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    /// <summary>
    /// Logique d'interaction pour TestClassView.xaml
    /// </summary>
    public partial class TestClassView : UserControl,IView<TestClassViewModel>, IViewClassDocument
    {
        public TestClassView()
        {
            InitializeComponent();
            DataContextChanged += TestClassView_DataContextChanged;
        }

        private void TestClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is TestClassViewModel oldVm)
                oldVm.PropertyChanged -= Vm_PropertyChanged;
            if (e.NewValue is TestClassViewModel vm)
            {
                XamlEditor.Text = vm.Xaml;
                CodeEditor.Text = vm.Cs;
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Xaml")
            //    XamlEditor.Text = ((TestClassViewModel) DataContext).Xaml;
            //if (e.PropertyName == "Code")
            //    CodeEditor.Text = ((TestClassViewModel) DataContext).Code;
        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, XamlEditor))
                ((TestClassViewModel) DataContext).Xaml = XamlEditor?.Text;
            if (ReferenceEquals(sender, CodeEditor))
                ((TestClassViewModel) DataContext).Cs = CodeEditor?.Text;
        }
    }
}
