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
    public abstract class TestForm : UserControlNotifier
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

        public virtual string Version => "";

        public virtual void Upgrade(FormValues values) { }

        private IEnumerable<FrameworkElement> _namedElements;

        private bool _allowProcess = false;

        private IForm Form
        {
            get
            {
                if (this is not IForm form) throw new Exception("TestForm should imlement IForm");
                return form;
            }
        }

        public void TryProcess(object sender, RoutedEventArgs args)
        {
            if(!_allowProcess) return;

            try
            {
                Form.Process(sender, args);
                SetFormMode(Mode);
            }
            catch (Exception e)
            {
                SetErrorMessage(new ExceptionView { DataContext = e });
            }
        }


        protected override void OnContentChanged(object oldContent, object newContent)
        {
            try
            {
                var form = Form;

                if (newContent is not FrameworkElement content) return;

                var checkboxes = new Dictionary<string, List<CheckBox>>();

                _namedElements = content.FindLogicalChildren<FrameworkElement>()
                    .Where(e => !string.IsNullOrWhiteSpace(e.Name)).ToList();

                var i = 0;

                foreach (var element in _namedElements)
                {
                    form.Connect(i, element);

                    switch (element)
                    {
                        case TextBoxEx textBoxEx:
                            textBoxEx.DoubleChange += form.TryProcess;
                            break;

                        case TextBox textBox:
                            textBox.TextChanged += form.TryProcess;
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
                                    form.TryProcess(sender, args);
                                };
                            }
                            else
                                checkBox.PreviewMouseDown += (a, f) =>
                            {
                                f.Handled = true;
                                form.TryProcess(a, f);
                            };

                            break;

                        case Button button:
                            button.Click += form.TryProcess;
                            break;
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
        private readonly IProperty<IFormTarget> _target = H<TestForm>.Property<IFormTarget>();

        public FormMode Mode
        {
            get => _mode.Get();
            set
            {
                if (_mode.Set(value))
                    SetFormMode(value);
            }
        }
        private readonly IProperty<FormMode> _mode = H<TestForm>.Property<FormMode>();



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



        private readonly ConcurrentBag<Action> _cache = new();

        public void Reset()
        {
            while (_cache.TryTake(out var action))
            {
                action();
            }
        }

        private void SetBrush(Control control, Brush brush)
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

        private void SetBrush(TextBlock control, Brush brush)
        {
            var oldBrush = control.Background;
            //var oldThickness = control.BorderThickness;
            control.Background = brush;
            //control.BorderThickness = OnThickness;
            _cache.Add(() =>
            {
                control.Background = oldBrush;
                //control.BorderThickness = oldThickness;
            });
        }

        public void NotConform(Control control)
        {
            Target.ConformityId = ConformityState.NotConform;
            SetBrush(control, NotConformBrush);
        }
        public void Conform(Control control)
        {
            Target.ConformityId = ConformityState.Conform;
            SetBrush(control, ConformBrush);
        }
        public void Invalid(Control control)
        {
            Target.ConformityId = ConformityState.Invalid;
            SetBrush(control, InvalidBrush);
        }
        public void NotConform(TextBlock control)
        {
            Target.ConformityId = ConformityState.NotConform;
            SetBrush(control, NotConformBrush);
        }
        public void Conform(TextBlock control)
        {
            Target.ConformityId = ConformityState.Conform;
            SetBrush(control, ConformBrush);
        }
        public void Invalid(TextBlock control)
        {
            Target.ConformityId = ConformityState.Invalid;
            SetBrush(control, InvalidBrush);
        }

        public abstract string GetPackedValues();
        public abstract string GetSpecPackedValues();

        protected string Sanitize(string s)
        {
            return s.Replace("■", "");
        }

        //protected virtual bool HasTag(FrameworkElement e, string tag)
        //{

        //}

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
            var specificationNeeded = 0;
            var mandatoryNeeded = 0;
            var optionalEmpty = 0;

            var specificationDone = 0;
            var mandatoryDone = 0;
            var optionalDone = 0;


            foreach (var element in _namedElements)
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
                if (spec) todo = () => specificationNeeded++;
                else if (mandatory) todo = () => mandatoryNeeded++;
                else todo = () => optionalEmpty++;

                Action done;
                if (spec) done = () => specificationDone++;
                else if (mandatory) done = () => mandatoryDone++;
                else done = () => optionalDone++;

                var enabled =
                    (spec && mode == FormMode.Specification)
                    || (!spec && mode == FormMode.Capture);

                switch (element)
                {
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
                }
            }


            // SPECIFICATION

            if (Target != null)
            {
                switch (mode)
                {
                    case FormMode.Specification:
                        {
                            Target.SpecificationValues = GetSpecPackedValues();
                            if (specificationNeeded > 0)
                            {
                                if (Target != null)
                                    Target.ConformityId = ConformityState.NotChecked;

                                Target.SpecificationDone = false;
                            }
                            else Target.SpecificationDone = true;

                            break;
                        }
                    case FormMode.Capture:
                        {
                            Target.ResultValues = GetPackedValues();
                            if (mandatoryNeeded > 0)
                            {
                                if (Target != null)
                                    Target.ConformityId = mandatoryDone > 0 ? ConformityState.Running : ConformityState.NotChecked;
                                Target.MandatoryDone = false;
                            }
                            else Target.MandatoryDone = true;

                            break;
                        }
                }

                if (Target.ConformityId > ConformityState.Running)
                {
                    if (specificationNeeded > 0) Target.ConformityId = ConformityState.NotChecked;
                    if (mandatoryNeeded > 0) Target.ConformityId = ConformityState.Running;
                }

                if (string.IsNullOrWhiteSpace(Target.Conformity))
                {
                    Target.Conformity = Target.ConformityId.Caption();
                }

                if (string.IsNullOrWhiteSpace(Target.TestName))
                {
                    Target.TestName = Target.DefaultTestName;
                }
            }
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

            if (values == null) return;
            var formValues = new FormValues(values);

            if(Form.Version != formValues.Version)
            {
                Form.Upgrade(formValues);
            }

            LoadValues(formValues);

            _allowProcess = allowProcess;
        }


        public void LoadValues(FormValues values)
        {
            foreach (var c in _namedElements)
            {
                var name = c.Name;
                var idx = name.IndexOf("__", StringComparison.Ordinal);
                var isBool = idx >= 0;
                if (isBool && c is CheckBox chk) name = name.Substring(0, idx);

                if (values.TryGetValue(name, out var value))
                    switch (c)
                    {
                        case TextBoxEx tbe:
                            tbe.Double = Csd(value);
                            break;
                        case TextBox tb:
                            tb.Text = value;
                            break;
                        case CheckBox cb:
                            if (isBool)
                            {
                                cb.IsChecked = c.Name == $"{name}__{value}";
                            }
                            else
                                cb.IsChecked = value switch
                                {
                                    "N" => null,
                                    "0" => false,
                                    "1" => true,
                                    _ => cb.IsChecked
                                };

                            break;
                    }
            }
        }
        public static double Csd(string str)
        {
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