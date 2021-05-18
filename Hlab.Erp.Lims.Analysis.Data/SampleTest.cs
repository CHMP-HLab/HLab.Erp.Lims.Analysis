using System;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<SampleTest>;


    public partial class SampleTest : Entity
        , IEntityWithIcon
        , IEntityWithColor
        ,IFormTarget
    {

        public SampleTest() => H.Initialize(this);

        public int? SampleId
        {
            get => _sample.Id.Get();
            set => _sample.Id.Set(value);
        }
        [Ignore]
        public virtual Sample Sample
        {
            set => SampleId = value.Id;
            get => _sample.Get();
        }
        private readonly IForeign<Sample> _sample = H.Foreign<Sample>();


        public int? TestClassId
        {
            get => _testClass.Id.Get();
            set => _testClass.Id.Set(value);
        }
        [Ignore]
        public virtual TestClass TestClass
        {
            set => TestClassId = value.Id;
            get => _testClass.Get();
        }
        private readonly IForeign<TestClass> _testClass = H.Foreign<TestClass>();


        public int? TestStateId
        {
            get => _testStateId.Get();
            set => _testStateId.Set(value);
        }
        private readonly IProperty<int?> _testStateId = H.Property<int?>();


        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }
        private readonly IProperty<int?> _userId = H.Property<int?>();


        public string Method { get; set; }

        public int? PurposeId
        {
            get => _purposeId.Get();
            set => _purposeId.Set(value);
        }
        private readonly IProperty<int?> _purposeId = H.Property<int?>();

        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>();

        public int? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }
        private readonly IProperty<int?> _validation = H.Property<int?>();


        public int? ValidatorId
        {
            get => _validatorId.Get();
            set => _validatorId.Set(value);
        }
        private readonly IProperty<int?> _validatorId = H.Property<int?>();

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }
        private readonly IProperty<string> _testName = H.Property<string>(c => c.Default(""));


        public string Version
        {
            get => _version.Get();
            set => _version.Set(value);
        }
        private readonly IProperty<string> _version = H.Property<string>(c => c.Default(""));


        //public byte[] Code
        //{
        //    get => _code.Get(); 
        //    set => _code.Set(value);
        //}
        //private readonly IProperty<byte[]> _code = H.Property<byte[]>();
        byte[] IFormTarget.Code => TestClass.Code;


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


        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }
        private readonly IProperty<string> _values = H.Property<string>(c => c.Default(""));

        string IFormTarget.SpecificationValues
        {
            get => Values; 
            set => Values = value;
        }

        public DateTime? ScheduledDate
        {
            get => _scheduledDate.Get();
            set => _scheduledDate.Set(value);
        }
        private readonly IProperty<DateTime?> _scheduledDate = H.Property<DateTime?>();

        public DateTime? StartDate
        {
            get => _startDate.Get();
            set => _startDate.Set(value);
        }
        private readonly IProperty<DateTime?> _startDate = H.Property<DateTime?>();


        public DateTime? EndDate
        {
            get => _endDate.Get();
            set => _endDate.Set(value);
        }
        private readonly IProperty<DateTime?> _endDate = H.Property<DateTime?>();


        public string OosNo
        {
            get => _oosNo.Get();
            set => _oosNo.Set(value);
        }
        private readonly IProperty<string> _oosNo = H.Property<string>(c => c.Default(""));

        public int? Order
        {
            get => _order.Get();
            set => _order.Set(value);
        }
        private readonly IProperty<int?> _order = H.Property<int?>();


        public string PharmacopoeiaVersion
        {
            get => _pharmacopoeiaVersion.Get();
            set => _pharmacopoeiaVersion.Set(value);
        }
        private readonly IProperty<string> _pharmacopoeiaVersion = H.Property<string>(c => c.Default(""));

        public int? PharmacopoeiaId
        {
            get => _pharmacopoeia.Id.Get();
            set => _pharmacopoeia.Id.Set(value);
        }

        [Ignore]
        public Pharmacopoeia Pharmacopoeia
        {
            get => _pharmacopoeia.Get();
            set => _pharmacopoeia.Set(value);
        }
        private readonly IForeign<Pharmacopoeia> _pharmacopoeia = H.Foreign<Pharmacopoeia>();

        [Column("Stage")]
        public string StageId
        {
            get => _stageId.Get();
            set => _stageId.Set(value);
        }
        private readonly IProperty<string> _stageId = H.Property<string>();
        
        [Ignore]
        public SampleTestWorkflow.Stage Stage
        {
            get => _stage.Get();
            set => StageId = value.Name;
        }
        private readonly IProperty<SampleTestWorkflow.Stage> _stage = H.Property<SampleTestWorkflow.Stage>(c => c
            .Set(e => SampleTestWorkflow.StageFromName(e.StageId))
            .On(e => e.StageId)
            .Update()
        );
        
        public bool SpecificationDone
        {
            get => _specificationDone.Get();
            set => _specificationDone.Set(value);
        }
        private readonly IProperty<bool> _specificationDone = H.Property<bool>();

        // RESULT
        public int? ResultId
        {
            get => _result.Id.Get();
            set => _result.Id.Set(value);
        }
        
        [Ignore]public SampleTestResult Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IForeign<SampleTestResult> _result = H.Foreign<SampleTestResult>();

        bool IFormTarget.MandatoryDone
        {
            get => Result?.MandatoryDone??true;
            set
            {
                if(Result!=null)
                    Result.MandatoryDone = value;
            }
        }

        string IFormTarget.Conformity
        {
            get => Result?.Conformity;
            set
            {
                if(Result!=null)
                    Result.Conformity = value;
            }
        }

        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }
        private readonly IProperty<double> _progress = H.Property<double>();

        [Ignore] string IFormTarget.Result
        {
            get => Result?.Result;
            set
            {
                if(Result!=null)
                    Result.Result = value;
            }
        }

        ConformityState IFormTarget.ConformityId 
        {
            get => Result?.ConformityId??ConformityState.NotChecked;
            set
            {
                if(Result!=null)
                    Result.ConformityId = value;
            }
        }

        string IFormTarget.ResultValues
        {
            get => Result?.Values;
            set
            {
                if(Result!=null)
                    Result.Values = value;
            }
        }

        // CALCULATED

        [Ignore]
        public int? Color => _color.Get();
        private readonly IProperty<int?> _color = H.Property<int?>(c => c
            .On(e => e.TestClass.Color)
            .Set(e => e.TestClass?.Color)
        );


        [Ignore]
        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .Set(e => string.IsNullOrWhiteSpace(e.TestClass?.IconPath) ? "icon/test/default" : e.TestClass.IconPath)
            .On(e => e.TestClass.IconPath).Update()
        );

        [Ignore]
        public ObservableQuery<SampleTestResult> Results => _results.Get();
        private readonly IProperty<ObservableQuery<SampleTestResult>> _results = H.Property<ObservableQuery<SampleTestResult>>(c => c
            .Foreign(e => e.SampleTestId)
        );


        [Ignore] string IFormTarget.DefaultTestName => TestClass?.Name;

        IFormClass IFormTarget.FormClass { get => TestClass; set => TestClass = (TestClass)value; }
        string IFormTarget.Name { get => TestClass?.Name; set => throw new NotImplementedException(); }

        public void Reset()
        {
            throw new NotImplementedException();
        }


    }
}
