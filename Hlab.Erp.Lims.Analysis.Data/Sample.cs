using System;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Sample>;
    public partial class Sample : Entity, IListableModel
    {
        public Sample() => H.Initialize(this);

        public string FileId
        {
            get => _fileId.Get();
            set => _fileId.Set(value);
        }
        private readonly IProperty<string> _fileId = H.Property<string>();


        public int? UserId
        {
            get => _user.Id.Get(); 
            set => _user.Id.Set(value);
        }
        [Ignore]
        public User User
        {
            get => _user.Get(); 
            set => _user.Set(value);
        }
        private readonly IForeign<User> _user = H.Foreign<User>(); 


        public string Reference
        {
            get => _reference.Get();
            set => _reference.Set(value);
        }
        private readonly IProperty<string> _reference = H.Property<string>();


        public string CustomerReference
        {
            get => _customerReference.Get();
            set => _customerReference.Set(value);
        }
        private readonly IProperty<string> _customerReference = H.Property<string>();


        public string ReportReference
        {
            get => _reportReference.Get();
            set => _reportReference.Set(value);
        }
        private readonly IProperty<string> _reportReference = H.Property<string>();


        public DateTime? ReceptionDate
        {
            get => _receptionDate.Get();
            set => _receptionDate.Set(value);
        }
        private readonly IProperty<DateTime?> _receptionDate = H.Property<DateTime?>();


        public string Worksheet

        {
            get => _worksheet.Get();
            set => _worksheet.Set(value);
        }
        private readonly IProperty<string> _worksheet = H.Property<string>(c => c.Default(""));


        public string CommercialName
        {
            get => _commercialName.Get();
            set => _commercialName.Set(value);
        }
        private readonly IProperty<string> _commercialName = H.Property<string>(c => c.Default(""));


        public string Batch
        {
            get => _batch.Get();
            set => _batch.Set(value);
        }
        private readonly IProperty<string> _batch = H.Property<string>(c => c.Default(""));


        public DateTime? ExpirationDate
        {
            get => _expirationDate.Get();
            set => _expirationDate.Set(value);
        }
        private readonly IProperty<DateTime?> _expirationDate = H.Property<DateTime?>();


        public bool ExpirationDayValid
        {
            get => _expirationDayValid.Get();
            set => _expirationDayValid.Set(value);
        }
        private readonly IProperty<bool> _expirationDayValid = H.Property<bool>();


        public DateTime? ManufacturingDate
        {
            get => _manufacturingDate.Get();
            set => _manufacturingDate.Set(value);
        }
        private readonly IProperty<DateTime?> _manufacturingDate = H.Property<DateTime?>();


        public bool ManufacturingDayValid
        {
            get => _manufacturingDayValid.Get();
            set => _manufacturingDayValid.Set(value);
        }
        private readonly IProperty<bool> _manufacturingDayValid = H.Property<bool>();
        
        
        public DateTime? SamplingDate
        {
            get => _samplingDate.Get();
            set => _samplingDate.Set(value);
        }
        private readonly IProperty<DateTime?> _samplingDate = H.Property<DateTime?>();


        public bool SamplingDayValid
        {
            get => _samplingDayValid.Get();
            set => _samplingDayValid.Set(value);
        }
        private readonly IProperty<bool> _samplingDayValid = H.Property<bool>();


        public string SamplingOrigin
        {
            get => _samplingOrigin.Get();
            set => _samplingOrigin.Set(value);
        }
        private readonly IProperty<string> _samplingOrigin = H.Property<string>(c => c.Default(""));


        public string PharmacopoeiaVersion
        {
            get => _pharmacopoeiaVersion.Get();
            set => _pharmacopoeiaVersion.Set(value);
        }
        private readonly IProperty<string> _pharmacopoeiaVersion = H.Property<string>(c => c.Default(""));


        public bool InOriginalPackaging
        {
            get => _inOriginalPackaging.Get();
            set => _inOriginalPackaging.Set(value);
        }
        private readonly IProperty<bool> _inOriginalPackaging = H.Property<bool>();


        public string PrimaryPackaging
        {
            get => _primaryPackaging.Get();
            set => _primaryPackaging.Set(value);
        }
        private readonly IProperty<string> _primaryPackaging = H.Property<string>(c => c.Default(""));


        public string SecondaryPackaging
        {
            get => _secondaryPackaging.Get();
            set => _secondaryPackaging.Set(value);
        }
        private readonly IProperty<string> _secondaryPackaging = H.Property<string>(c => c.Default(""));


        public double? ReceivedQuantity
        {
            get => _receivedQuantity.Get();
            set => _receivedQuantity.Set(value);
        }
        private readonly IProperty<double?> _receivedQuantity = H.Property<double?>();


        public string Aspect
        {
            get => _aspect.Get();
            set => _aspect.Set(value);
        }
        private readonly IProperty<string> _aspect = H.Property<string>(c => c.Default(""));


        public string Size
        {
            get => _size.Get();
            set => _size.Set(value);
        }
        private readonly IProperty<string> _size = H.Property<string>(c => c.Default(""));


        public bool HasInstruction
        {
            get => _hasInstruction.Get();
            set => _hasInstruction.Set(value);
        }
        private readonly IProperty<bool> _hasInstruction = H.Property<bool>();


        public bool NoticeFr
        {
            get => _noticeFr.Get();
            set => _noticeFr.Set(value);
        }
        private readonly IProperty<bool> _noticeFr = H.Property<bool>();


        public bool NoticeEn
        {
            get => _noticeEn.Get();
            set => _noticeEn.Set(value);
        }
        private readonly IProperty<bool> _noticeEn = H.Property<bool>();


        public string InstructionLanguages
        {
            get => _instructionLanguages.Get();
            set => _instructionLanguages.Set(value);
        }
        private readonly IProperty<string> _instructionLanguages = H.Property<string>(c => c.Default(""));


        public string StorageConditions
        {
            get => _storageConditions.Get();
            set => _storageConditions.Set(value);
        }
        private readonly IProperty<string> _storageConditions = H.Property<string>(c => c.Default(""));


        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));


        public string Conclusion
        {
            get => _conclusion.Get();
            set => _conclusion.Set(value);
        }
        private readonly IProperty<string> _conclusion = H.Property<string>(c => c.Default(""));


         public DateTime? NotificationDate
        {
            get => _notificationDate.Get();
            set => _notificationDate.Set(value);
        }
        private readonly IProperty<DateTime?> _notificationDate = H.Property<DateTime?>();


        public int? ValidatorId
        {
            get => _validator.Id.Get();
            set => _validator.Id.Set(value);
        }


        [Ignore]
        public User Validator
        {
            get => _validator.Get();
            set => ValidatorId = value?.Id;
        }
        private readonly IForeign<User> _validator = H.Foreign<User>();


        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }
        private readonly IProperty<double> _progress = H.Property<double>();


        public sbyte? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }
        private readonly IProperty<sbyte?> _validation = H.Property<sbyte?>();


        public ConformityState ConformityId
        {
            get => _conformityId.Get();
            set => _conformityId.Set(value);
        }
        private readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>();

        [Column("Stage")]
        public string StageId
        {
            get => _stageId.Get();
            set => _stageId.Set(value);
        }
        private readonly IProperty<string> _stageId = H.Property<string>();

        [Ignore]
        public SampleWorkflow.Stage Stage
        {
            get => _stage.Get();
            set => StageId = value.Name;
        }
        private readonly IProperty<SampleWorkflow.Stage> _stage = H.Property<SampleWorkflow.Stage>(c => c
            .Set(e => SampleWorkflow.StageFromName(e.StageId))
            .On(e => e.StageId)
            .Update()
        );


        public int? CustomerId
        {
            get => _customer.Id.Get();
            set => _customer.Id.Set(value);
        }
        [Ignore]
        public Customer Customer
        {
            get => _customer.Get();
            set => CustomerId = value.Id;
        }
        private readonly IForeign<Customer> _customer = H.Foreign<Customer>();


        public int? ManufacturerId
        {
            get => _manufacturer.Id.Get();
            set => _manufacturer.Id.Set(value);
        }
        [Ignore]
        public virtual Manufacturer Manufacturer
        {
            get => _manufacturer.Get();
            set => _manufacturer.Set(value);
        }
        private readonly IForeign<Manufacturer> _manufacturer = H.Foreign<Manufacturer>();


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


        public int? ProductId
        {
            get => _product.Id.Get();
            set => _product.Id.Set(value);
        }
        [Ignore]
        public Product Product
        {
            get => _product.Get();
            set => _product.Set(value);
        }
        private readonly IForeign<Product> _product = H.Foreign<Product>( );

        public int? AnalysisMotivationId
        {
            get => _analysisMotivation.Id.Get();
            set => _analysisMotivation.Id.Set(value);
        }
        [Ignore]
        public AnalysisMotivation AnalysisMotivation
        {
            get => _analysisMotivation.Get();
            set => _analysisMotivation.Set(value);
        }
        private readonly IForeign<AnalysisMotivation> _analysisMotivation = H.Foreign<AnalysisMotivation>( );

        [Ignore] public ObservableQuery<SampleTest> SampleTests => _sampleTests.Get();
        private readonly IProperty<ObservableQuery<SampleTest>> _sampleTests = H.Property<ObservableQuery<SampleTest>>(c => c
            .Foreign(e => e.SampleId)
        );

        
        [Ignore]
        public static Sample DesignModel => new Sample
        {
                Reference = "0042/11/2019",
                ReceivedQuantity = 100,
                ReceptionDate = DateTime.Now,
                Progress = 50,
                User = User.DesignModel

        };

        public bool Invoiced
        {
            get => _invoiced.Get();
            set => _invoiced.Set(value);
        }
        private readonly IProperty<bool> _invoiced = H.Property<bool>();


        public bool Paid
        {
            get => _paid.Get();
            set => _paid.Set(value);
        }
        private readonly IProperty<bool> _paid = H.Property<bool>();


        public string InvoiceNo
        {
            get => _invoiceNo.Get();
            set => _invoiceNo.Set(value);
        }
        private readonly IProperty<string> _invoiceNo = H.Property<string>();

        [Ignore]
        public bool Expired => _expired.Get();
        private readonly IProperty<bool> _expired = H.Property<bool>(c =>
            c.On(e => e.ExpirationDate).Set(e =>
            {
                if (e.ExpirationDate == null) return false;
                return DateTime.Now > e.ExpirationDate;
            }));

        [Ignore]
        public TimeSpan Life => _life.Get();
        private readonly IProperty<TimeSpan> _life = H.Property<TimeSpan>(c => c
            .On(e => e.ExpirationDate)
            .On(e => e.ManufacturingDate)
            .NotNull(e => e.ExpirationDate)
            .NotNull(e => e.ManufacturingDate)
            .Set(e =>
            {
                if(e.ExpirationDate == null) return new TimeSpan(0);
                if(e.ManufacturingDate == null) return new TimeSpan(0);

                return e.ExpirationDate.Value - e.ManufacturingDate.Value;
            }));

        [Ignore]
        public bool EndOfLife => _endOfLife.Get();
        private readonly IProperty<bool> _endOfLife = H.Property<bool>(c => c
            .On(e => e.ExpirationDate)
            .NotNull(e => e.ExpirationDate)
            .Set(e =>
            {
                if (e.ExpirationDate == null) return false;
                return DateTime.Now > e.ExpirationDate.Value.Subtract(new TimeSpan(e.Life.Ticks / 3));
            }));

        [Ignore]
        public string Caption => Reference;

        [Ignore]
        public string IconPath => "Icons/Entities/Sample";
    }
}
