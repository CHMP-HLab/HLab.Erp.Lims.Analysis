using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Notify.Wpf;
using YAMP;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestLegacyForm>;

    public class TestLegacyForm : UserControlNotifier
    {
        protected static class TestEtat
        {
            public static ConformityState Indefini => ConformityState.Undefined;
            public static ConformityState NonCommence => ConformityState.NotChecked;
            public static ConformityState EnCours => ConformityState.Running;
            public static ConformityState NonConforme => ConformityState.NotConform;
            public static ConformityState Conforme => ConformityState.Conform;
            public static ConformityState Invalide => ConformityState.Invalid;
        }

        public TestLegacyForm()
        {
            Test = new LegacyHelper(this);
            H.Initialize(this);
        }

        public class LegacyHelper
        {
            private TestLegacyForm _form;
            public LegacyHelper(TestLegacyForm form)
            {
                _form = form;
            }
            public ConformityState Etat
            {
                get => _form.Target.ConformityId;
                set => _form.Target.ConformityId = value;
            }

            public string Conforme
            {
                get => _form.Target.Conformity;
                set => _form.Target.Conformity = value;
            }

            public string Norme
            {
                get => _form.Target.Specification;
                set => _form.Target.Specification = value;
            }

            public string NomTest
            {
                get => _form.Target.TestName;
                set => _form.Target.TestName = value;
            }

            public string Description
            {
                get => _form.Target.Description;
                set => _form.Target.Description = value;
            }

            public string Resultat
            {
                get => _form.Target.Result;
                set => _form.Target.Result = value;
            }

            public double Calcul(TextBlock block, double value, int decimals = 2) =>
                _form.Compute(block, value, decimals);

            public double EvalueFormule(string formula) => Evaluate(formula);

            public static void CheckGroupe(object sender, params CheckBox[] group) => CheckGroup(sender, group);

        }
        protected LegacyHelper Test { get; }

        public IFormTarget Target 
        {
            get => _target.Get();
            set => _target.Set(value);
        }
        private readonly IProperty<IFormTarget> _target = H.Property<IFormTarget>();

        public double Compute(TextBlock block, double value, int decimals = 2)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                block.Background = Brushes.LightPink;
                block.Text = "!";
                return double.NaN;
            }

            block.Background = Brushes.Transparent;
            block.Text = Math.Round(value, decimals).ToString();
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

            if (sender is CheckBox ck)
            {
                if (ck.IsChecked == true)
                {
                    foreach (var c in @group)
                    {
                        c.IsChecked = null;
                    }
                }
                else
                {
                    ck.IsChecked = true;
                    foreach (var c in @group.Where(e => e != ck))
                    {
                        c.IsChecked = false;
                    }
                }
            }
        }

        private static readonly Brush ConformBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush NotConformBrush = new SolidColorBrush(Colors.Red);
        private static readonly Brush InvalidBrush = new SolidColorBrush(Colors.Yellow);

        private static readonly Thickness OnThickness = new(0.4);

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
            SetBrush(control,NotConformBrush);
        }
        public void Conform(Control control)
        {
            Target.ConformityId = ConformityState.Conform;
            SetBrush(control,ConformBrush);
        }
        public void Invalid(Control control)
        {
            Target.ConformityId = ConformityState.Invalid;
            SetBrush(control,InvalidBrush);
        }
        public void NotConform(TextBlock control)
        {
            Target.ConformityId = ConformityState.NotConform;
            SetBrush(control,NotConformBrush);
        }
        public void Conform(TextBlock control)
        {
            Target.ConformityId = ConformityState.Conform;
            SetBrush(control,ConformBrush);
        }
        public void Invalid(TextBlock control)
        {
            Target.ConformityId = ConformityState.Invalid;
            SetBrush(control,InvalidBrush);
        }
    }
}