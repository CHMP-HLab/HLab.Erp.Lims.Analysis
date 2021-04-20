using HLab.Notify.PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.FormClasses;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestLegacyForm>;

    public abstract class TestLegacyForm : TestForm
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

        protected TestLegacyForm()
        {
            Test = new LegacyHelper(this);
            H.Initialize(this);
        }

        protected override bool HasLevel(FrameworkElement e,ElementLevel level)
        {
            if (e.Tag is not string tag || string.IsNullOrWhiteSpace(tag)) return base.HasLevel(e, level);
            tag = tag.ToLower();
            return level switch
            {
                ElementLevel.Specification => tag[0] == 'n' || base.HasLevel(e, level),
                ElementLevel.Mandatory => tag[0] == 'o' || base.HasLevel(e, level),
                _ => base.HasLevel(e, level)
            };
        }

        public class LegacyHelper
        {
            private readonly TestLegacyForm _form;
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
                Compute(block, value, decimals);

            public double EvalueFormule(string formula) => Evaluate(formula);

            public void CheckGroupe(object sender, params CheckBox[] group) => CheckGroup(sender, group);

        }
        protected LegacyHelper Test { get; }

    }
}