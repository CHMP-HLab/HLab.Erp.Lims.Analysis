using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

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
                oldVm.FormHelper.PropertyChanged -= Vm_PropertyChanged;
            if (e.NewValue is TestClassViewModel vm)
            {
                XamlEditor.Text = vm.FormHelper.Xaml;
                CodeEditor.Text = vm.FormHelper.Cs;
                vm.FormHelper.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("XamlError"))
            {
                HiglightError(
                    ((TestClassViewModel) DataContext).FormHelper.XamlErrorLine,
                    ((TestClassViewModel) DataContext).FormHelper.XamlErrorPos
                );
            }
            //    XamlEditor.Text = ((TestClassViewModel) DataContext).Xaml;
            //if (e.PropertyName == "Code")
            //    CodeEditor.Text = ((TestClassViewModel) DataContext).Code;

        }

        class MarkError : DocumentColorizingTransformer
        {
            private readonly int _line;
            private readonly int _pos;
            public MarkError(int line, int pos)
            {
                _line = line;
                _pos = pos;
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (line.LineNumber == _line)
                {
                    if (_pos<line.Length)
                        ChangeLinePart(line.Offset+_pos-1,line.EndOffset,e => e.TextRunProperties.SetBackgroundBrush(Brushes.LightPink));
                }
            }
        }


        private void HiglightError(int line, int pos)
        {
            foreach (var markSameWord in XamlEditor.TextArea.TextView.LineTransformers.OfType<MarkError>().ToList())
            {
                XamlEditor.TextArea.TextView.LineTransformers.Remove(markSameWord);
            }

            if(line>-1 && pos>-1)
                XamlEditor.TextArea.TextView.LineTransformers.Add(new MarkError(line,pos));

        }

        private void TextEditor_OnTextChanged(object sender, EventArgs e)
        {
            if (ReferenceEquals(sender, XamlEditor))
                ((TestClassViewModel) DataContext).FormHelper.Xaml = XamlEditor?.Text;
            if (ReferenceEquals(sender, CodeEditor))
                ((TestClassViewModel) DataContext).FormHelper.Cs = CodeEditor?.Text;
        }
    }

    
}
