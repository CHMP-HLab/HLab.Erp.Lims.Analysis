using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AvalonDock.Controls;
using HLab.Base.Wpf;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Notify.PropertyChanged;
using HLab.Notify.Wpf;
using YAMP;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public abstract class TestForm : UserControlNotifier
    {
        private static readonly Brush ConformBrush = Brushes.Green;
        private static readonly Brush NotConformBrush = Brushes.Red;
        private static readonly Brush InvalidBrush = Brushes.LightPink;
        private static readonly Brush ValidBrush = Brushes.Transparent;
        private static readonly Thickness OnThickness = new(0.4);


        private static readonly Brush NormalBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
        private static readonly Brush SpecificationNeededBrush = Brushes.MediumSpringGreen;
        private static readonly Brush SpecificationDoneBrush = Brushes.DarkGreen;
        private static readonly Brush MandatoryBrush = Brushes.PaleVioletRed;
        private static readonly Brush HiddenBrush = Brushes.Black;


        protected TestForm()
        {
            H<TestForm>.Initialize(this);
        }

        private IEnumerable<FrameworkElement> _namedElements;

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            try
            {
                if (this is not IForm form) return;

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
                            textBoxEx.DoubleChange += (sender, args) =>
                            {
                                form.Process(sender, args);
                                SetFormMode(Mode);
                            };
                            break;

                        case TextBox textBox:
                            textBox.TextChanged += (sender, args) =>
                            {
                                form.Process(sender, args);
                                SetFormMode(Mode);
                            };
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
                                    TestLegacyForm.CheckGroup(sender, chk.ToArray());
                                    form.Process(sender, args);
                                    SetFormMode(Mode);
                                };
                            }
                            else
                                checkBox.PreviewMouseDown += (sender, args) =>
                                {
                                    //args.Handled = true;
                                    form.Process(sender, args);
                                    SetFormMode(Mode);
                                };

                            break;

                        case Button button:
                            button.Click += (sender, args) =>
                            {
                                form.Process(sender, args);
                                SetFormMode(Mode);
                            };
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
                if(_mode.Set(value))
                    SetFormMode(value);
            }
        }
        private readonly IProperty<FormMode> _mode = H<TestForm>.Property<FormMode>();

        public static double Compute(TextBlock block, double value, int decimals = 2)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                block.Background = InvalidBrush;
                block.Text = "!";
                return double.NaN;
            }

            block.Background = ValidBrush;
            block.Text = Math.Round(value, decimals).ToString(CultureInfo.CurrentCulture);
            return value;
        }

        public static double Evaluate(string formula)
        {
            try
            {
                var parser = new Parser();

                var result = parser.Evaluate(formula);

                if (result != null)
                    return ((ScalarValue)result).Value;
            }
            catch { }

            return double.NaN;
        }

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

        protected virtual bool HasLevel(FrameworkElement e,ElementLevel level)
        {
            switch (level)
            {
                case ElementLevel.Capture:
                    return e is TextBox || e is CheckBox;
                case ElementLevel.Result:
                    return e is TextBlock || e is CheckBox;
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
                        ?spec?SpecificationDoneBrush:HiddenBrush
                        :spec?SpecificationDoneBrush:NormalBrush                        
                        ;

                    var todoBrush = 
                        mode == FormMode.Specification
                        ?spec?SpecificationNeededBrush:HiddenBrush
                        :mandatory?MandatoryBrush:NormalBrush;


                    Action todo;
                    if (spec) todo = () => specificationNeeded++;
                    else if(mandatory) todo = () => mandatoryNeeded++;
                    else todo = () => optionalEmpty++;

                    Action done;
                    if (spec) done = () => specificationDone++;
                    else if(mandatory) done = () => mandatoryDone++;
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
                            if(Target!=null)
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
                            if(Target!=null)
                                Target.ConformityId = mandatoryDone > 0 ? ConformityState.Running : ConformityState.NotChecked;
                            Target.MandatoryDone = false;
                        }
                        else Target.MandatoryDone = true;

                        break;
                    }
                }

                if(Target.ConformityId>ConformityState.Running)
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

        public void LoadValues(string values)
        {
            if (values == null) return;
            var dict = new Dictionary<string, string>();

            foreach (var value in values.Split('■'))// Le séparateur est un ALT + 254
            {
                var v = value.Split("=");
                if (v.Length > 1)
                {
                    dict.TryAdd(v[0],v[1]);
                }
                else if (v.Length == 1)
                {
                    dict.TryAdd(v[0],"");
                }

            }

            LoadValues(dict);

            SetFormMode(Mode);
        }
        public void LoadValues(Dictionary<string,string> values)
        {
            foreach (var c in _namedElements)
            {
                var name = c.Name;
                var idx = name.IndexOf("__", StringComparison.Ordinal);
                var isBool = idx >= 0;
                if (isBool && c is CheckBox chk) name = name.Substring(0, idx);

                if(values.TryGetValue(name,out var value))
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
                                cb.IsChecked = (c.Name == name + "__" + value);
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

    }
}