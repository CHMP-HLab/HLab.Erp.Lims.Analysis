using System;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<SampleTest>;
    public partial class SampleTest : Entity
        , IEntityWithIcon
        , IEntityWithColor
    {

        public SampleTest() => H.Initialize(this);

        public int? SampleId
        {
            get => _sampleId.Get();
            set => _sampleId.Set(value);
        }
        private readonly IProperty<int?> _sampleId = H.Property<int?>();


        public int? TestClassId
        {
            get => _testClassId.Get();
            set => _testClassId.Set(value);
        }
        private readonly IProperty<int?> _testClassId = H.Property<int?>();


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


        //[Column]
        //public DateTime? DateValidation
        //{
        //    get => N.Get(() => (DateTime?)null); set => N.Set(value);
        //}


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


        //[StringLength(16777215)]
        //public string Cs1 { get; set; }

        //[StringLength(16777215)]
        //public string Xaml1 { get; set; }

        public byte[] Code
        {
            get => _code.Get(); 
            set => _code.Set(value);
        }
        private readonly IProperty<byte[]> _code = H.Property<byte[]>();

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


        public string Conform
        {
            get => _conform.Get();
            set => _conform.Set(value);
        }
        private readonly IProperty<string> _conform = H.Property<string>(c => c.Default(""));

        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }
        private readonly IProperty<string> _values = H.Property<string>(c => c.Default(""));

        public int? ResultId
        {
            get => _result.Id.Get();
            set => _result.Id.Set(value);
        }

        [Ignore]
        public SampleTestResult Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IForeign<SampleTestResult> _result = H.Foreign<SampleTestResult>();

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


        public bool? ReTest
        {
            get => _reTest.Get();
            set => _reTest.Set(value);
        }
        private readonly IProperty<bool?> _reTest = H.Property<bool?>();


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

        //[LOG 24]\nNom=Etape\nId=0 : Saisie du test\nId=1 : Test normé\nId=2 : A faire\nId=3 : En cours\nId=4 : Validé par le technicien\nId=5 : Données brutes validées\nId=6 : Validé par le pharmacien
        public string Stage
        {
            get => _stage.Get();
            set => _stage.Set(value);
        }
        private readonly IProperty<string> _stage = H.Property<string>();
        public bool SpecificationsDone
        {
            get => _specificationsDone.Get();
            set => _specificationsDone.Set(value);
        }
        private readonly IProperty<bool> _specificationsDone = H.Property<bool>();

        [Ignore]
        public virtual Sample Sample
        {
            //get => E.GetForeign<Sample>(() => SampleId);
            set => SampleId = value.Id;
            get => _sample.Get();
            //set => _sample.Set(value);
        }
        private readonly IProperty<Sample> _sample = H.Property<Sample>(c => c.Foreign(e => e.SampleId));

        [Ignore]
        public virtual TestClass TestClass
        {
            set => TestClassId = value.Id;
            get => _testClass.Get();
        }
        private readonly IProperty<TestClass> _testClass = H.Property<TestClass>(c => c
            .Foreign(e => e.TestClassId)
        );

        [Ignore]
        public int? Color => _color.Get();
        private readonly IProperty<int?> _color = H.Property<int?>(c => c
            .On(e => e.TestClass.Color)
            .Set(e => e.TestClass?.Color)
        );


        [Ignore]
        public string IconPath => _iconPath.Get();
        private IProperty<string> _iconPath = H.Property<string>(c => c
            .OneWayBind(e => e.TestClass.IconPath)
        );

        [Ignore]
        public ObservableQuery<SampleTestResult> Results => _results.Get();
        private readonly IProperty<ObservableQuery<SampleTestResult>> _results = H.Property<ObservableQuery<SampleTestResult>>(c => c
            .Foreign(e => e.SampleTestId)
        );

        //[Ignore]
        //[Import]
        //public ObservableQuery<TestResult> TestResults
        //{
        //    get => N.Get<ObservableQuery<TestResult>>();
        //    set => N.Set(value.AddFilter("OneToMany", e => e.SampleTestId == Id)
        //        .FluentUpdate());
        //}
    }
}
