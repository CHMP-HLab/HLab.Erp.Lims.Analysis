using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Compiler.Wpf;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    /// <summary>
    /// Logique d'interaction pour TestClassView.xaml
    /// </summary>
    public partial class FormClassView : UserControl, IView<IFormHelperProvider>, 
        IViewClassDocument
    {
        public FormClassView()
        {
            InitializeComponent();

            DataContextChanged += TestClassView_DataContextChanged;
        }

        void TestClassView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IFormHelperProvider oldVm)
                oldVm.FormHelper.PropertyChanged -= FormHelper_PropertyChanged;
            if (e.NewValue is IFormHelperProvider vm)
            {
                vm.FormHelper.PropertyChanged += FormHelper_PropertyChanged;
            }
        }

        void FormHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            readonly int _line;
            readonly int _pos;

            public MarkError(int line, int pos)
            {
                _line = line;
                _pos = pos;
            }

            protected override void ColorizeLine(DocumentLine line)
            {
                if (line.LineNumber == _line)
                {
                    if (_pos < line.Length)
                        ChangeLinePart(line.Offset + _pos - 1, line.EndOffset,
                            e => e.TextRunProperties.SetBackgroundBrush(Brushes.LightPink));
                }
            }
        }

        void DeleteErrors(TextEditor editor)
        {
            foreach (var markSameWord in editor.TextArea.TextView.LineTransformers.OfType<MarkError>().ToList())
            {
                editor.TextArea.TextView.LineTransformers.Remove(markSameWord);
            }
        }

        void HighlightError(TextEditor editor, CompileError error)
        {
            DeleteErrors(editor);
            if (error == null) return;

            if (error.Line > -1 && error.Pos > -1)
                editor.TextArea.TextView.LineTransformers.Add(new MarkError(error.Line, error.Pos));

            editor.TextArea.Caret.Line = error.Line;
            editor.TextArea.Caret.BringCaretToView();
        }

    }
}
