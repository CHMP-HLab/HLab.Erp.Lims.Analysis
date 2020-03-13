using System;
using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
     public partial class SampleTestResult : Entity<SampleTestResult>
//        , IEntityWithIcon
//        , IEntityWithColor
    {
        //public int? Color => _color.Get();
        //private readonly IProperty<int?> _color = H.Property<int?>(c => c.OneWayBind(e => e.SampleTest.TestClass.Color));

        //public string IconName => _iconName.Get();
        //private readonly IProperty<string> _iconName = H.Property<string>(c => c.OneWayBind(e => e.SampleTest.TestClass.IconName));

        public int? SampleTestId
        {
            get => _sampleTestId.Get();
            set => _sampleTestId.Set(value);
        }

        private readonly IProperty<int?> _sampleTestId = H.Property<int?>();

        [Ignore]
        public virtual SampleTest SampleTest
        {
            get => _sampleTest.Get();
            set => SampleTestId = value?.Id;
        }

        private readonly IProperty<SampleTest> _sampleTest = H.Property<SampleTest>(c => c.Foreign(e => e.SampleTestId));

        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }

        private readonly IProperty<int?> _userId = H.Property<int?>();

        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }
        private readonly IProperty<string> _values = H.Property<string>(c => c.Default(""));

        public string Result
        {
            get => _result.Get(); set => _result.Set(value);
        }

        readonly IProperty<string> _result = H.Property<string>(c => c.Default(""));
        public string Conformity
        {
            get => _conformity.Get(); set => _conformity.Set(value);
        }

        readonly IProperty<string> _conformity = H.Property<string>(c => c.Default(""));

        public DateTime? Start
        {
            get => _start.Get(); 
            set => _start.Set(value);
        }
        readonly IProperty<DateTime?> _start = H.Property<DateTime?>();

        public DateTime? End
        {
            get => _end.Get(); 
            set => _end.Set(value);
        }
        readonly IProperty<DateTime?> _end = H.Property<DateTime?>();

        public int? StateId
        {
            get => _stateId.Get();
            set => _stateId.Set(value);
        }
        private readonly IProperty<int?> _stateId = H.Property<int?>();
        public int? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }
        private readonly IProperty<int?> _validation = H.Property<int?>();
        public string Stage
        {
            get => _stage.Get();
            set => _stage.Set(value);
        }
        private readonly IProperty<string> _stage = H.Property<string>();
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get();
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H.Property<bool>();
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>();
    }
}
