using System;
using System.Collections.Generic;
using System.Linq;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;
using Org.BouncyCastle.Crypto.Operators;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<TestClassUnitTest>;

    public class TestClassUnitTest : Entity, IFormTarget
    {
        public TestClassUnitTest() => H.Initialize(this);

        public int? TestClassId
        {
            get => _testClass.Id.Get();
            set => _testClass.Id.Set(value);
        }
        [Ignore] public TestClass TestClass
        {
            get => _testClass.Get();
            set => _testClass.Set(value);
        }
        private readonly IForeign<TestClass> _testClass = H.Foreign<TestClass>();
        [Ignore] IFormClass IFormTarget.FormClass 
        {
            get => TestClass;
            set => TestClass = (TestClass)value;
        }


        [Ignore] byte[] IFormTarget.Code => TestClass?.Code;
        [Ignore] string IFormTarget.DefaultTestName => TestClass?.Name;

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
