using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HLab.Erp.Lims.Analysis.Data;
using YAMP;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestLegacyHelper>;

    //public enum TestEtat
    //{
    //    Indefini = -1,
    //    NonCommence = 0,
    //    EnCours = 1,
    //    NonConforme = 2,
    //    Conforme = 3
    //};    

    public static class TestEtat
    {
        public static ConformityState Indefini => ConformityState.Undefined;
        public static ConformityState NonCommence => ConformityState.NotChecked;
        public static ConformityState EnCours => ConformityState.Running;
        public static ConformityState NonConforme => ConformityState.NotConform;
        public static ConformityState Conforme => ConformityState.Conform;
        public static ConformityState Invalide => ConformityState.Invalid;
    }


    public interface ITestHelper : INotifyPropertyChanged
    {
        ConformityState State { get; set; }
        string Conformity { get; set; }
        string Specifications { get; set; }
        string TestName { get; set; }
        string Description { get; set; }
        string Result { get; set; }

        void Reset();
        //bool SpecificationsDone { get; set; }
        //bool MandatoryDone { get; set; }
    }

    public class TestLegacyHelper : NotifierBase, ITestHelper
    {
        public TestLegacyHelper() => H.Initialize(this);

        public ConformityState State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private readonly IProperty<ConformityState> _state = H.Property<ConformityState>();

        public ConformityState Etat
        {
            get => _etat.Get();
            set => State = value;
        }
        private readonly IProperty<ConformityState> _etat = H.Property<ConformityState>(c => c.Bind(e => e.State));

        public string Conforme
        {
            get => _conforme.Get();
            set => Conformity = value;
        }
        private readonly IProperty<string> _conforme = H.Property<string>(c => c.Bind(e => e.Conformity));

        public string Norme
        {
            get => _norme.Get();
            set => Specifications = value;
        }
        private readonly IProperty<string> _norme = H.Property<string>(c => c.Bind(e => e.Specifications));

        public string NomTest
        {
            get => _nomTest.Get();
            set => TestName = value;
        }
        private readonly IProperty<string> _nomTest = H.Property<string>(c => c.Bind(e => e.TestName));

        public string Description
        {
            get => _description.Get();
            set => _description.Set(value);
        }
        private readonly IProperty<string> _description = H.Property<string>();

        public string Resultat
        {
            get => _resultat.Get();
            set => Result = value;
        }
        private readonly IProperty<string> _resultat = H.Property<string>(c => c.Bind(e => e.Result));


        public string Conformity
        {
            get => _conformity.Get();
            set => _conformity.Set(value);
        }
        private readonly IProperty<string> _conformity = H.Property<string>();

        public string Specifications
        {
            get => _specifications.Get();
            set => _specifications.Set(value);
        }
        private readonly IProperty<string> _specifications = H.Property<string>();

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }
        private readonly IProperty<string> _testName = H.Property<string>();

        public string Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<string> _result = H.Property<string>();

        public double Calcul(TextBlock block, double value, int decimals = 2) =>
            Compute(block, value, decimals);
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

        public double EvalueFormule(string formula) => Evaluate(formula);
        public double Evaluate(string formula)
        {
            try
            {
                var parser = new Parser();

                Value result = parser.Evaluate(formula);


                if (result != null)
                    return ((ScalarValue)result).Value;
            }
            catch { }

            return double.NaN;
        }

        public static void CheckGroupe(object sender, params CheckBox[] group) => CheckGroup(sender, group);
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
            State = ConformityState.NotConform;
            SetBrush(control,NotConformBrush);
        }
        public void Conform(Control control)
        {
            State = ConformityState.Conform;
            SetBrush(control,ConformBrush);
        }
        public void Invalid(Control control)
        {
            State = ConformityState.Invalid;
            SetBrush(control,InvalidBrush);
        }
        public void NotConform(TextBlock control)
        {
            State = ConformityState.NotConform;
            SetBrush(control,NotConformBrush);
        }
        public void Conform(TextBlock control)
        {
            State = ConformityState.Conform;
            SetBrush(control,ConformBrush);
        }
        public void Invalid(TextBlock control)
        {
            State = ConformityState.Invalid;
            SetBrush(control,InvalidBrush);
        }
    }
}