using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Conformity.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<TestClassUnitTestClone>;

    public static class TestClassUnitTestExtension
    {
        public static bool Check(this IFormTarget reference, IFormTarget test, out string error)
        {
            var spec = CompareValues(reference.SpecificationValues, test.SpecificationValues);
            var value = CompareValues(reference.ResultValues, test.ResultValues);

            var result = spec.Concat(value).ToList();

            if (result.Count == 0)
            {
                error = "";
                return true;
            }
            error = string.Join("\r\n",result);
            return false;
        }

        private class Entry
        {
            public string Name;
            public string Reference;
            public string Value;

            public override string ToString() => $@"{Name} = {Reference} <> {Value}";
        }

        private static IEnumerable<Entry> CompareValues(string v1, string v2)
        {
            var a1 = v1.Split("■").ToHashSet();
            var a2 = v2.Split("■").ToHashSet();

            foreach (var v in a1.ToList())
            {
                if (a2.Contains(v))
                {
                    a1.Remove(v);
                    a2.Remove(v);
                }
            }

            var d = new Dictionary<string, Entry>();
            foreach (var v in a1)
            {
                var k = v.Split("=");
                if (k.Length == 2)
                {
                    if (d.ContainsKey(k[0]))
                    {
                        d[k[0]].Name = k[0];
                        d[k[0]].Reference = k[1];
                    }
                    else d.Add(k[0],new Entry{Reference = k[1]});
                }
            }
            foreach (var v in a2)
            {
                var k = v.Split("=");
                if (k.Length == 2)
                {
                    if (d.ContainsKey(k[0]))
                    {
                        d[k[0]].Name = k[0];
                        d[k[0]].Value = k[1];
                    }
                    else d.Add(k[0],new Entry{Value = k[1]});
                }
            }

            return d.Values;
        }

        public static void Load(this IFormTarget target, IFormTarget source)
        {
            target.FormClass = source.FormClass;
            target.Name = source.Name;

            target.SpecificationValues = source.SpecificationValues;
            target.ResultValues = source.ResultValues;

            target.SpecificationDone = source.SpecificationDone;
            target.MandatoryDone = source.MandatoryDone;

            target.TestName = source.TestName;
            target.Description = source.Description;
            target.Specification = source.Specification;
            target.Result = source.Result;
            target.Conformity = source.Conformity;
            target.ConformityId = source.ConformityId;
        }

        public static T Clone<T>(this IFormTarget source) where T : IFormTarget, new()
        {
            return new T
            {
                FormClass = source.FormClass,
                Name = source.Name,
                SpecificationValues = source.SpecificationValues,
                ResultValues = source.ResultValues,
                SpecificationDone = source.SpecificationDone,
                MandatoryDone = source.MandatoryDone,
                TestName = source.TestName,
                Description = source.Description,
                Specification = source.Specification,
                Result = source.Result,
                Conformity = source.Conformity,
                ConformityId = source.ConformityId
            };
        }
    }

    public class TestClassUnitTestClone : NotifierBase, IFormTarget
    {
        public TestClassUnitTestClone() => H.Initialize(this);

        public IFormClass FormClass
        {
            get => _formClass.Get();
            set => _formClass.Set(value);
        }
        private readonly IProperty<IFormClass> _formClass = H.Property<IFormClass>();


        [Ignore] byte[] IFormTarget.Code => FormClass?.Code;
        [Ignore] string IFormTarget.DefaultTestName => FormClass?.Name;

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string ResultValues
        {
            get => _resultValues.Get();
            set => _resultValues.Set(value);
        }
        private readonly IProperty<string> _resultValues = H.Property<string>(c => c.Default(""));

        public string SpecificationValues
        {
            get => _specificationValues.Get();
            set => _specificationValues.Set(value);
        }
        private readonly IProperty<string> _specificationValues = H.Property<string>(c => c.Default(""));

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }

        private readonly IProperty<string> _testName = H.Property<string>(c => c.Default(""));

        public string Description
        {
            get => _description.Get();
            set => _description.Set(value);
        }
        private readonly IProperty<string> _description = H.Property<string>(c => c.Default(""));

        public string Specification
        {
            get => _specification.Get();
            set => _specification.Set(value);
        }
        private readonly IProperty<string> _specification = H.Property<string>(c => c.Default(""));
        
        public bool SpecificationDone
        {
            get => _specificationDone.Get();
            set => _specificationDone.Set(value);
        }
        private readonly IProperty<bool> _specificationDone = H.Property<bool>(c => c.Default(false));

        public string Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<string> _result = H.Property<string>(c => c.Default(""));

        public ConformityState ConformityId
        {
            get => _conformityId.Get();
            set => _conformityId.Set(value);
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        private readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>(c => c.Default(ConformityState.NotChecked));

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get();
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H.Property<bool>(c => c.Default(false));

        public string Conformity
        {
            get => _conformity.Get();
            set => _conformity.Set(value);
        }
        private readonly IProperty<string> _conformity = H.Property<string>(c => c.Default(""));






        //public string Conform
        //{
        //    get => _conform.Get();
        //    set => _conform.Set(value);
        //}
        //private readonly IProperty<string> _conform = H.Property<string>(c => c.Default(""));

        //public string State
        //{
        //    get => _state.Get();
        //    set => _state.Set(value);
        //}
        //private readonly IProperty<string> _state = H.Property<string>(c => c.Default(""));
    }
}
