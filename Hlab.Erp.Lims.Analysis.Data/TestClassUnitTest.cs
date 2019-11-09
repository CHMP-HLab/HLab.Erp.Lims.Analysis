using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace HLab.Erp.Lims.Analysis.Data
{
    class TestClassUnitTest : Entity<TestClassUnitTest>
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }
        readonly IProperty<string> _values = H.Property<string>(c => c.Default(""));

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }
        readonly IProperty<string> _testName = H.Property<string>(c => c.Default(""));

        public string Description
        {
            get => _description.Get();
            set => _description.Set(value);
        }
        readonly IProperty<string> _description = H.Property<string>(c => c.Default(""));

        public string Specifications
        {
            get => _specifications.Get();
            set => _specifications.Set(value);
        }

        readonly IProperty<string> _specifications = H.Property<string>(c => c.Default(""));
        public string Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        readonly IProperty<string> _result = H.Property<string>(c => c.Default(""));

        public string State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        readonly IProperty<string> _state = H.Property<string>(c => c.Default(""));
    }
}
