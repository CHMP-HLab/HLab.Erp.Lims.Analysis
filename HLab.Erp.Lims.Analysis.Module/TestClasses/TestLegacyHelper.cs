using HLab.Notify.PropertyChanged;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using YAMP;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public enum TestEtat
    {
        Indefini = -1,
        NonCommence = 0,
        EnCours = 1,
        NonConforme = 2,
        Conforme = 3
    };    
    
    public interface ITestHelper : INotifyPropertyChanged
    {
        string State { get; }
        string Conformity { get; }
        string Specifications { get; }
        string TestName { get; }
        string Description { get; }
        string Result { get; }
    }

    public class TestLegacyHelper : N<TestLegacyHelper>, ITestHelper
    {
        public TestEtat Etat 
        { 
            get => (TestEtat)Enum.Parse(typeof(TestEtat),State); 
            set => State = value.ToString(); 
        }
        public string Conforme
        { 
            get => _conforme.Get();
            set => Conformity = value; 
        }
        private IProperty<string> _conforme = H.Property<string>(c => c.OneWayBind(e => e.Conformity));

        public string Norme
        { 
            get => _norme.Get();
            set => Specifications = value; 
        }
        private IProperty<string> _norme = H.Property<string>(c => c.OneWayBind(e => e.Specifications));

        public string NomTest 
        {
            get => _nomTest.Get();
            set => TestName = value;
        }
        private IProperty<string> _nomTest = H.Property<string>(c => c.OneWayBind(e => e.TestName));

        public string Description
        {
            get => _description.Get();
            set => _description.Set(value);
        }
        private IProperty<string> _description = H.Property<string>();

        public string Resultat
        {
            get => _resultat.Get();
            set => Result = value;
        }
        private IProperty<string> _resultat = H.Property<string>(c => c.OneWayBind(e => e.Result));

        public string State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private IProperty<string> _state = H.Property<string>();

        public string Conformity
        {
            get => _conformity.Get();
            set => _conformity.Set(value);
        }
        private IProperty<string> _conformity = H.Property<string>();

        public string Specifications
        {
            get => _specifications.Get();
            set => _specifications.Set(value);
        }
        private IProperty<string> _specifications = H.Property<string>();

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }
        private IProperty<string> _testName = H.Property<string>();

    public string Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
    private IProperty<string> _result = H.Property<string>();


    public double Calcul(TextBlock textblock, double valeur, int nbDecimales = 2)
        {
            if (double.IsInfinity(valeur) || double.IsNaN(valeur))
            {
                textblock.Background = Brushes.LightPink;
                textblock.Text = "!";
                return double.NaN;
            }

            textblock.Background = Brushes.Transparent;
            textblock.Text = Math.Round(valeur, nbDecimales).ToString();
            return valeur;
        }
        public double EvalueFormule(String formule)
        {
            try
            {
                var parser = new Parser();

                Value result = parser.Evaluate(formule);


                if (result != null)
                    return ((ScalarValue)result).Value;
            }
            catch { }

            return double.NaN;
        }
        public void CheckGroupe(object sender, params CheckBox[] group)
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