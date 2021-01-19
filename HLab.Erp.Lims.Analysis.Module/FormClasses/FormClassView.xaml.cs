using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    /// <summary>
    /// Logique d'interaction pour TestClassView.xaml
    /// </summary>
    public partial class FormClassView : UserControl,IView<FormClassViewModel>, IViewClassDocument
    {
        public FormClassView()
        {
            InitializeComponent();
            DataContextChanged += TestClassView_DataContextChanged;
        }

        private void TestClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue is FormClassViewModel oldVm)
                oldVm.FormHelper.PropertyChanged -= Vm_PropertyChanged;
            if (e.NewValue is FormClassViewModel vm)
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
                    ((FormClassViewModel) DataContext).FormHelper.XamlErrorLine,
                    ((FormClassViewModel) DataContext).FormHelper.XamlErrorPos
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
                ((FormClassViewModel) DataContext).FormHelper.Xaml = XamlEditor?.Text;
            if (ReferenceEquals(sender, CodeEditor))
                ((FormClassViewModel) DataContext).FormHelper.Cs = CodeEditor?.Text;
        }
    }

    
}
