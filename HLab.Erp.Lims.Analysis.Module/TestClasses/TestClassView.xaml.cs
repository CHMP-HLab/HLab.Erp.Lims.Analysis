using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Compiler.Wpf;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.CodeAnalysis;

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
                oldVm.FormHelper.PropertyChanged -= FormHelper_PropertyChanged;
            if (e.NewValue is TestClassViewModel vm)
            {
                vm.FormHelper.PropertyChanged += FormHelper_PropertyChanged;
            }
        }

        private void FormHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not FormHelper helper) return;
            switch (e.PropertyName)
            {
                case nameof(helper.SelectedXamlError):
                    HighlightError(XamlEditor, helper.SelectedXamlError);
                    break;
                case nameof(helper.SelectedCsError):
                {
                    HighlightError(CodeEditor, helper.SelectedCsError);
                    break;
                }
                case nameof(helper.SelectedDebugError): 
                    HighlightError(FinalCodeEditor, helper.SelectedDebugError);
                    break;
            }
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

        private void DeleteErrors(TextEditor editor)
        {
            foreach (var markSameWord in editor.TextArea.TextView.LineTransformers.OfType<MarkError>().ToList())
            {
                editor.TextArea.TextView.LineTransformers.Remove(markSameWord);
            }
        }

        private void HighlightError(TextEditor editor, CompileError error)
        {
            DeleteErrors(editor);
            if (error == null) return;

            if(error.Line>-1 && error.Pos>-1)
                editor.TextArea.TextView.LineTransformers.Add(new MarkError(error.Line,error.Pos));

            editor.TextArea.Caret.Line = error.Line;
            editor.TextArea.Caret.BringCaretToView();
        }

        //private void TextEditor_OnTextChanged(object sender, EventArgs e)
        //{
        //    if (DataContext is not TestClassViewModel vm) return;
        //    if (ReferenceEquals(sender, XamlEditor))
        //    {
        //        if (vm.FormHelper.Xaml != XamlEditor.Text)
        //        {
        //            vm.FormHelper.Xaml = XamlEditor.Text;
        //            vm.TryAllowed = true;
        //        }
        //    }

        //    if (ReferenceEquals(sender, CodeEditor))
        //    {
        //        if (vm.FormHelper.Cs != CodeEditor.Text)
        //        {
        //            vm.FormHelper.Cs = CodeEditor.Text;
        //            vm.TryAllowed = true;
        //        }
        //    }

        //}
    }

    
}
