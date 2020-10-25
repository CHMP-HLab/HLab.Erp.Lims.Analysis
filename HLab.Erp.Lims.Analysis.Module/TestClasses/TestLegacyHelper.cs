using HLab.Notify.PropertyChanged;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
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
    public enum TestState
    {
        Undefined = -1,
        NotStarted = 0,
        Running = 1,
        NotConform = 2,
        Conform = 3,
        NotValid = 4
    };

    public static class TestEtat
    {
        public static TestState Indefini => TestState.Undefined;
        public static TestState NonCommence => TestState.NotStarted;
        public static TestState EnCours => TestState.Running;
        public static TestState NonConforme => TestState.NotConform;
        public static TestState Conforme => TestState.Conform;
        public static TestState Invalide => TestState.NotValid;
    }


    public interface ITestHelper : INotifyPropertyChanged
    {
        TestState State { get; set; }
        string Conformity { get; set; }
        string Specifications { get; set; }
        string TestName { get; set; }
        string Description { get; set; }
        string Result { get; set; }
        //bool SpecificationsDone { get; set; }
        //bool MandatoryDone { get; set; }
    }

    public class TestLegacyHelper : NotifierBase, ITestHelper
    {
        public TestLegacyHelper() => H.Initialize(this);

        public TestState State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private readonly IProperty<TestState> _state = H.Property<TestState>();

        public TestState Etat
        {
            get => _etat.Get();
            set => State = value;
        }
        private readonly IProperty<TestState> _etat = H.Property<TestState>(c => c.OneWayBind(e => e.State));

        public string Conforme
        {
            get => _conforme.Get();
            set => Conformity = value;
        }
        private readonly IProperty<string> _conforme = H.Property<string>(c => c.OneWayBind(e => e.Conformity));

        public string Norme
        {
            get => _norme.Get();
            set => Specifications = value;
        }
        private readonly IProperty<string> _norme = H.Property<string>(c => c.OneWayBind(e => e.Specifications));

        public string NomTest
        {
            get => _nomTest.Get();
            set => TestName = value;
        }
        private readonly IProperty<string> _nomTest = H.Property<string>(c => c.OneWayBind(e => e.TestName));

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
        private readonly IProperty<string> _resultat = H.Property<string>(c => c.OneWayBind(e => e.Result));


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

        //public bool SpecificationsDone
        //{
        //    get => _specificationsDone.Get();
        //    set => _specificationsDone.Set(value);
        //}
        //private IProperty<bool> _specificationsDone = H.Property<bool>();
        //public bool MandatoryDone
        //{
        //    get => _mandatoryDone.Get();
        //    set => _mandatoryDone.Set(value);
        //}
        //private IProperty<bool> _mandatoryDone = H.Property<bool>();

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

        public void CheckGroupe(object sender, params CheckBox[] group) => CheckGroup(sender, group);
        public void CheckGroup(object sender, params CheckBox[] group)
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
    }
}