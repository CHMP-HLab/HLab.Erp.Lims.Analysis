using System;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<SampleTestResult>;


    public partial class SampleTestResult : Entity, IFormTarget
    //        , IEntityWithIcon
    //        , IEntityWithColor
    {
        public SampleTestResult() => H.Initialize(this);


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
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<string> _result = H.Property<string>(c => c.Default(""));


        public string Conformity
        {
            get => _conformity.Get();
            set => _conformity.Set(value);
        }
        private readonly IProperty<string> _conformity = H.Property<string>(c => c.Default(""));

        public DateTime? Start
        {
            get => _start.Get();
            set => _start.Set(value);
        }
        private readonly IProperty<DateTime?> _start = H.Property<DateTime?>();

        public DateTime? End
        {
            get => _end.Get();
            set => _end.Set(value);
        }

        private readonly IProperty<DateTime?> _end = H.Property<DateTime?>();


        public ConformityState ConformityId
        {
            get => _conformityId.Get();
            set => _conformityId.Set(value);
        }

        void IFormTarget.Reset()
        {
            throw new NotImplementedException();
        }

        private readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>();


        public int? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }
        private readonly IProperty<int?> _validation = H.Property<int?>();

        [Column("Stage")]
        public string StageId
        {
            get => _stageId.Get();
            set => _stageId.Set(value);
        }
        private readonly IProperty<string> _stageId = H.Property<string>();

        [Ignore]
        public SampleTestResultWorkflow.Stage Stage
        {
            get => _stage.Get();
            set => StageId = value.Name;
        }
        private readonly IProperty<SampleTestResultWorkflow.Stage> _stage = H.Property<SampleTestResultWorkflow.Stage>(c => c
            .Set(e => SampleTestResultWorkflow.StageFromName(e.StageId))
            .On(e => e.StageId)
            .Update()
        );

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();


        string IFormTarget.ResultValues
        {
            get => Values;
            set => Values = value;
        }

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

        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }
        private readonly IProperty<double> _progress = H.Property<double>();

        // TEST

        [Ignore] string IFormTarget.Description
        {
            get => SampleTest.Description;
            set => SampleTest.Description = value;
        }

        [Ignore] string IFormTarget.TestName
        {
            get => SampleTest.TestName;
            set => SampleTest.TestName = value;
        }
        byte[] IFormTarget.Code => SampleTest.TestClass.Code;

        string IFormTarget.SpecificationValues
        {
            get => SampleTest.Values;
            set => SampleTest.Values = value;
        }

        bool IFormTarget.SpecificationDone
        {
            get => SampleTest.SpecificationDone;
            set => SampleTest.SpecificationDone = value;
        }
        string IFormTarget.Specification
        {
            get => SampleTest.Specification;
            set => SampleTest.Specification = value;
        }

        string IFormTarget.DefaultTestName => ((IFormTarget)SampleTest).DefaultTestName;

        IFormClass IFormTarget.FormClass { get => SampleTest.TestClass; set => throw new NotImplementedException(); }
    }
}
