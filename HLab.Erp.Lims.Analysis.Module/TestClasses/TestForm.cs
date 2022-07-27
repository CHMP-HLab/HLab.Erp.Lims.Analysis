using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using HLab.Base.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Notify.PropertyChanged;
using HLab.Notify.Wpf;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public abstract class TestForm : UserControlNotifier, IForm
    {
        protected static readonly Brush ConformBrush = Brushes.Green;
        protected static readonly Brush NotConformBrush = Brushes.Red;
        protected static readonly Brush InvalidBrush = Brushes.LightPink;
        protected static readonly Brush ValidBrush = Brushes.Transparent;
        protected static readonly Thickness OnThickness = new(0.4);


        protected static readonly Brush NormalBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
        protected static readonly Brush SpecificationNeededBrush = Brushes.MediumSpringGreen;
        protected static readonly Brush SpecificationDoneBrush = Brushes.DarkGreen;
        protected static readonly Brush MandatoryBrush = Brushes.PaleVioletRed;
        protected static readonly Brush HiddenBrush = Brushes.Black;


        protected TestForm()
        {
            H<TestForm>.Initialize(this);
        }

        public IEnumerable<FrameworkElement> NamedElements {get; set;}

        bool _allowProcess = false;

        //private IForm Form
        //{
        //    get
        //    {
        //        if (this is IForm form) return form;
        //        throw new Exception("TestForm should imlement IForm");

        //    }
        //}


        ConformityState _conformityState = ConformityState.NotChecked;
        bool _reenteringTest = false;
        public void TryProcess(object sender, RoutedEventArgs args)
        {
            if (!_allowProcess) return;

            if(_reenteringTest) {}
            _reenteringTest = true;

            try
            { 
                SetErrorMessage(new TextBlock { Text="Ok" });

                _conformityState = ConformityState.NotChecked;
                ResetBrush();

                ((IForm)this).Process(sender, args);

                Target.ConformityId = _conformityState;
                SetFormMode(Mode);
            }
            catch (Exception e)
            {
                SetErrorMessage(new ExceptionView { DataContext = e });
            }
            finally
            {
                _reenteringTest = false;
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            try
            {
                //var form = Form;

                if (newContent is not FrameworkElement content) return;

                var checkboxes = new Dictionary<string, List<CheckBox>>();

                var i = 0;

                foreach (var element in NamedElements)
                {
                    ((IForm)this).Connect(i, element);

                    switch (element)
                    {
                        case DoubleBox doubleBox:
                            if(!doubleBox.IsReadOnly)
                                doubleBox.DoubleChanged += TryProcess;
                            break;

                        case TextBoxEx textBoxEx:
                            if(!textBoxEx.IsReadOnly)
                                textBoxEx.TextChanged += TryProcess;
                            break;

                        case TextBox textBox:
                            if(!textBox.IsReadOnly)
                                textBox.TextChanged += TryProcess;
                            break;

                        case CheckBox checkBox:
                            if (checkBox.Name.Contains("__"))
                            {
                                var pos = checkBox.Name.LastIndexOf("__", StringComparison.InvariantCulture);
                                var name = checkBox.Name.Substring(0, pos);

                                if (!checkboxes.TryGetValue(name, out var chk))
                                {
                                    chk = new List<CheckBox>();
                                    checkboxes.Add(name, chk);
                                }

                                chk.Add(checkBox);

                                checkBox.PreviewMouseDown += (sender, args) =>
                                {
                                    args.Handled = true;
                                    CheckGroup(sender, chk.ToArray());
                                    TryProcess(sender, args);
                                };
                            }
                            else
                            {
                                checkBox.Checked += TryProcess;

                                checkBox.Unchecked += TryProcess;

                            }

                            break;

                        case Button button:
                            button.Click += TryProcess;
                            break;

                            //case ComboBox cbo:
                            //    cbo.SelectionChanged += form.TryProcess;
                            //    break;
                    }
                    i++;
                }
            }
            finally
            {
                base.OnContentChanged(oldContent, newContent);
            }
        }

        public IFormTarget Target
        {
            get => _target.Get();
            set => _target.Set(value);
        }

        public long CreationDuration { get; set; }

        readonly IProperty<IFormTarget> _target = H<TestForm>.Property<IFormTarget>();

        public FormMode Mode
        {
            get => _mode.Get();
            set
            {
                if (_mode.Set(value))
                    SetFormMode(value);
            }
        }

        readonly IProperty<FormMode> _mode = H<TestForm>.Property<FormMode>();



        public static void CheckGroup(object sender, params CheckBox[] group)
        {
            if (!group.Contains(sender)) return;

            if (sender is not CheckBox ck) return;

            if (ck.IsChecked == true)
            {
                Check(@group, null);
            }
            else
            {
                ck.IsChecked = true;
                Check(@group.Where(e => e != ck), false);
            }

            static void Check(IEnumerable<CheckBox> checkBoxes, bool? value)
            {
                foreach (var c in checkBoxes)
                {
                    c.IsChecked = value;
                }
            }
        }


        readonly ConcurrentBag<Action> _cache = new();

        public void ResetBrush()
        {
            while (_cache.TryTake(out var action))
            {
                action();
            }
        }

        void SetBrush(FrameworkElement element, Brush brush)
        {
            if (element is Control control)
                SetBrush(control, brush);
            else
            if (element is TextBlock textBlock)
                SetBrush(textBlock, brush);

        }

        void SetBrush(Control control, Brush brush)
        {
            var oldBrush = control.BorderBrush;
            var oldThickness = control.BorderThickness;
            control.BorderBrush = brush;
            control.BorderThickness = OnThickness;
            _cache.Add(() =>
            {
                control.BorderBrush = oldBrush;
                control.BorderThickness = oldThickness;
            });
        }

        void SetBrush(TextBlock control, Brush brush)
        {
            var oldBrush = control.Background;
            control.Background = brush;
            _cache.Add(() =>
            {
                control.Background = oldBrush;
            });
        }

        public void NotConform(params FrameworkElement[] elements)
        {
            switch (_conformityState)
            {
                case ConformityState.NotChecked:
                    _conformityState = ConformityState.NotConform;
                    break;
                case ConformityState.Running:
                    _conformityState = ConformityState.NotConform;
                    break;
                case ConformityState.NotConform:
                    break;
                case ConformityState.Conform:
                    _conformityState = ConformityState.NotConform;
                    break;
                case ConformityState.Invalid:
                    break;
            }
            foreach (var element in elements) SetBrush(element, NotConformBrush);
        }

        public void Conform(params FrameworkElement[] elements)
        {
            switch (_conformityState)
            {
                case ConformityState.NotChecked:
                    _conformityState = ConformityState.Conform;
                    break;
                case ConformityState.Running:
                    break;
                case ConformityState.NotConform:
                    break;
                case ConformityState.Conform:
                    break;
                case ConformityState.Invalid:
                    break;
            }

            foreach (var element in elements) SetBrush(element, ConformBrush);
        }

        public void Invalid(params FrameworkElement[] elements)
        {
            switch (_conformityState)
            {
                case ConformityState.NotChecked:
                    _conformityState = ConformityState.Invalid;
                    break;
                case ConformityState.Running:
                    break;
                case ConformityState.NotConform:
                    _conformityState = ConformityState.Invalid;
                    break;
                case ConformityState.Conform:
                    _conformityState = ConformityState.Invalid;
                    break;
                case ConformityState.Invalid:
                    break;
            }
            foreach (var element in elements) SetBrush(element, InvalidBrush);
        }
        public void Running(params FrameworkElement[] elements)
        {
            switch (_conformityState)
            {
                case ConformityState.NotChecked:
                    _conformityState = ConformityState.Running;
                    break;
                case ConformityState.Running:
                    break;
                case ConformityState.NotConform:
                    _conformityState = ConformityState.Running;
                    break;
                case ConformityState.Conform:
                    _conformityState = ConformityState.Running;
                    break;
                case ConformityState.Invalid:
                    _conformityState = ConformityState.Running;
                    break;
            }
            foreach (var element in elements) SetBrush(element, InvalidBrush);
        }

        public abstract string GetPackedValues();
        public abstract string GetSpecPackedValues();

        protected string Sanitize(string s)
        {
            return s.Replace("■", "");
        }

        protected virtual bool HasLevel(FrameworkElement e, ElementLevel level)
        {
            switch (level)
            {
                case ElementLevel.Capture:
                    return e is TextBox or CheckBox;
                case ElementLevel.Result:
                    return e is TextBlock or CheckBox;
            }

            if (e.Tag is not string tag || string.IsNullOrWhiteSpace(tag)) return false;
            switch (level)
            {
                case ElementLevel.Specification:
                    return tag[0] == 's';
                case ElementLevel.Mandatory:
                    return tag[0] == 'm';
            }

            return false;
        }


        public void SetFormMode(FormMode mode)
        {
            var todoSpecification = 0;
            var todoMandatory = 0;
            var todoOptional = 0;

            var specificationDone = 0;
            var mandatoryDone = 0;
            var optionalDone = 0;

            List<string> todoList = new();


            foreach (var element in NamedElements)
            {
                var spec = HasLevel(element, ElementLevel.Specification);
                var mandatory = HasLevel(element, ElementLevel.Mandatory);

                var doneBrush =
                    mode == FormMode.Specification
                    ? spec ? SpecificationDoneBrush : HiddenBrush
                    : spec ? SpecificationDoneBrush : NormalBrush
                    ;

                var todoBrush =
                    mode == FormMode.Specification
                    ? spec ? SpecificationNeededBrush : HiddenBrush
                    : mandatory ? MandatoryBrush : NormalBrush;


                Action todo;

                if (spec) todo = () => todoSpecification++;
                else if (mandatory)
                {
                    todo = () =>
                    {
                        todoMandatory++;
                        todoList.Add(element.Name);
                    };
                }
                else todo = () => todoOptional++;

                Action done;
                if (spec) done = () => specificationDone++;
                else if (mandatory) done = () => mandatoryDone++;
                else done = () => optionalDone++;

                var enabled =
                    (spec && mode == FormMode.Specification)
                    || (!spec && mode == FormMode.Capture);

                switch (element)
                {
                    // DoubleBox
                    case DoubleBox doubleBox when Math.Abs(doubleBox.Double) > double.Epsilon:
                        doubleBox.Background = doneBrush;
                        doubleBox.IsEnabled = enabled;
                        done();
                        break;

                    case DoubleBox doubleBox:
                        doubleBox.Background = todoBrush;
                        doubleBox.IsEnabled = enabled;
                        todo();
                        break;

                    // TextBoxEx
                    case TextBoxEx tb when Math.Abs(tb.Double) > double.Epsilon:
                        tb.Background = doneBrush;
                        tb.IsEnabled = enabled;
                        done();
                        break;

                    case TextBoxEx tb:
                        tb.Background = todoBrush;
                        tb.IsEnabled = enabled;
                        todo();
                        break;

                    // TextBox
                    case TextBox tb when tb.Text.Length > 0:
                        tb.Background = doneBrush;
                        tb.IsEnabled = enabled;
                        done();
                        break;

                    case TextBox tb:
                        tb.Background = todoBrush;
                        tb.IsEnabled = enabled;
                        todo();
                        break;

                    // CheckBox
                    case CheckBox cb when cb.IsChecked != null:
                        cb.Background = doneBrush;
                        cb.IsEnabled = enabled;
                        done();
                        break;

                    case CheckBox cb:
                        cb.Background = todoBrush;
                        cb.IsEnabled = enabled;
                        todo();
                        break;

                    case ComboBox cbo when cbo.Text.Length > 0:
                        cbo.Background = doneBrush;
                        cbo.IsEnabled = enabled;
                        done();
                        break;

                    case ComboBox cbo:
                        cbo.Background = todoBrush;
                        cbo.IsEnabled = enabled;
                        todo();
                        break;

                }
            }


            // SPECIFICATION

            if (Target != null)
            {
                if (mode == FormMode.Specification)
                {
                    //Store specification values
                    try
                    {
                        Target.SpecificationValues = $"{GetSpecPackedValues()}Version={((IForm)this).Version}■";
                    }
                    catch(Exception e)
                    {
                        SetErrorMessage(new ExceptionView { DataContext = e });
                    }
                    if (todoSpecification > 0)
                    {
                        Target.ConformityId = ConformityState.NotChecked;
                        Target.SpecificationDone = false;
                    }
                    else Target.SpecificationDone = true;

                    if (string.IsNullOrWhiteSpace(Target.TestName))
                    {
                        Target.TestName = Target.DefaultTestName;
                    }
                }
                else if (mode == FormMode.Capture)
                {
                    //Store result values
                    try
                    {
                        Target.ResultValues = $"{GetPackedValues()}Version={((IForm)this).Version}■";
                    }
                    catch(Exception e)
                    {
                        SetErrorMessage(new ExceptionView { DataContext = e });
                    }
                    if (todoMandatory > 0)
                    {
                        Target.ConformityId = mandatoryDone > 0 ? ConformityState.Running : ConformityState.NotChecked;
                        Target.MandatoryDone = false;
                    }
                    else Target.MandatoryDone = true;

                    if (string.IsNullOrWhiteSpace(Target.Conformity) || GetConformities().Contains(Target.Conformity))
                    {
                        Target.Conformity = Target.ConformityId.Caption();
                    }
                }

            }
        }

        static IEnumerable<string> GetConformities()
        {
            yield return ConformityState.Running.Caption();
            yield return ConformityState.Conform.Caption();
            yield return ConformityState.Invalid.Caption();
            yield return ConformityState.NotChecked.Caption();
            yield return ConformityState.NotConform.Caption();
        }

        public bool PreventProcess()
        {
            var old = _allowProcess;
            _allowProcess = false;
            return old;
        }

        public void AllowProcess()
        {
            _allowProcess = true;
        }

        public void LoadValues(string values)
        {
            var allowProcess = PreventProcess();

            if (string.IsNullOrWhiteSpace(values)) return;

            var formValues = new FormValues(values);

            if (((IForm)this).Version != formValues.Version)
            {
                ((IForm)this).Upgrade(formValues);
            }

            LoadValues(formValues);

            _allowProcess = allowProcess;
        }


        public void LoadValues(FormValues values)
        {
            foreach (var c in NamedElements)
            {
                var name = c.Name;
                var idx = name.IndexOf("__", StringComparison.Ordinal);
                var isBool = idx >= 0;
                if (isBool && c is CheckBox chk) name = name.Substring(0, idx);

                if (values.TryGetValue(name, out var value))
                    switch (c)
                    {
                        case DoubleBox doubleBox:
                            doubleBox.Double = Csd(value);
                            break;

                        case TextBoxEx textBoxEx:
                            textBoxEx.Double = Csd(value);
                            break;

                        case TextBox textBox:
                            textBox.Text = value;
                            break;

                        case CheckBox checkBox:
                            if (isBool)
                            {
                                checkBox.IsChecked = c.Name == $"{name}__{value}";
                            }
                            else
                                checkBox.IsChecked = value switch
                                {
                                    "N" => null,
                                    "0" => false,
                                    "1" => true,
                                    _ => checkBox.IsChecked
                                };

                            break;
                    }
            }
        }
        public static double Csd(string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return 0.0;

            str = str.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".", StringComparison.InvariantCulture);
            try
            {
                return Convert.ToDouble(str, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public void SetErrorMessage(FrameworkElement fe)
        {
            ToolTip = fe;
        }
    }
}