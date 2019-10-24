using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using YAMP;

namespace HLab.Erp.Lims.Analysis.Module.AssayClasses
{
    public enum TestEtat
    {
        Indefini = -1,
        NonCommence = 0,
        EnCours = 1,
        NonConforme = 2,
        Conforme = 3
    };    
    
    public class AssayLegacyHelper
    {
        public TestEtat Etat { get; set; }
        public string Conforme { get; set; }
        public string Norme { get; set; } 
        public string NomTest { get; set; }  
        public string Description { get; set; }  
        public string Resultat { get; set; } 

        public double Calcul(TextBlock textblock, double valeur, int nbDecimales = 2)
        {
            if(double.IsInfinity(valeur) || double.IsNaN(valeur))
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


                if(result != null)
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
                if(ck.IsChecked == true)
                {
                    foreach (var c in @group)
                    {
                        c.IsChecked = null;
                    }
                }
                else
                {
                    ck.IsChecked = true;
                    foreach (var c in @group.Where(e => e!=ck))
                    {
                        c.IsChecked = false;
                    }
                }

            }
        }
    }
}