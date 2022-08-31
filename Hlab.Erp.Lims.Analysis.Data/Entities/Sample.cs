using System;
using HLab.Base.Extensions;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Entities
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
        readonly IProperty<string> _fileId = H.Property<string>();

        public int? UserId
        {
            get => _user.Id.Get(); 
            set => _user.Id.Set(value);
        }
        public User User
        {
            get => _user.Get(); 
            set => _user.Set(value);
        }
        readonly IForeign<User> _user = H.Foreign<User>(); 

        public string Reference
        {
            get => _reference.Get();
            set => _reference.Set(value);
        }
        readonly IProperty<string> _reference = H.Property<string>();

        public string CustomerReference
        {
            get => _customerReference.Get();
            set => _customerReference.Set(value);
        }
        readonly IProperty<string> _customerReference = H.Property<string>();

        public string ReportReference
        {
            get => _reportReference.Get();
            set => _reportReference.Set(value);
        }

        readonly IProperty<string> _reportReference = H.Property<string>();


        public DateTime? ReceptionDate
        {
            get => _receptionDate.Get().ToUniversalTime();
            set => _receptionDate.Set(value.ToUniversalTime());
        }

        readonly IProperty<DateTime?> _receptionDate = H.Property<DateTime?>();


        public string Worksheet

        {
            get => _worksheet.Get();
            set => _worksheet.Set(value);
        }

        readonly IProperty<string> _worksheet = H.Property<string>(c => c.Default(""));


        public string CommercialName
        {
            get => _commercialName.Get();
            set => _commercialName.Set(value);
        }

        readonly IProperty<string> _commercialName = H.Property<string>(c => c.Default(""));


        public string Batch
        {
            get => _batch.Get();
            set => _batch.Set(value);
        }

        readonly IProperty<string> _batch = H.Property<string>(c => c.Default(""));


        public DateTime? ExpirationDate
        {
            get => _expirationDate.Get().ToUniversalTime();
            set => _expirationDate.Set(value.ToUniversalTime());
        }

        readonly IProperty<DateTime?> _expirationDate = H.Property<DateTime?>();


        public bool ExpirationDayValid
        {
            get => _expirationDayValid.Get();
            set => _expirationDayValid.Set(value);
        }

        readonly IProperty<bool> _expirationDayValid = H.Property<bool>();


        public DateTime? ManufacturingDate
        {
            get => _manufacturingDate.Get().ToUniversalTime();
            set => _manufacturingDate.Set(value.ToUniversalTime());
        }

        readonly IProperty<DateTime?> _manufacturingDate = H.Property<DateTime?>();


        public bool ManufacturingDayValid
        {
            get => _manufacturingDayValid.Get();
            set => _manufacturingDayValid.Set(value);
        }

        readonly IProperty<bool> _manufacturingDayValid = H.Property<bool>();
        
        
        public DateTime? SamplingDate
        {
            get => _samplingDate.Get().ToUniversalTime();
            set => _samplingDate.Set(value.ToUniversalTime());
        }

        readonly IProperty<DateTime?> _samplingDate = H.Property<DateTime?>();


        public bool SamplingDayValid
        {
            get => _samplingDayValid.Get();
            set => _samplingDayValid.Set(value);
        }

        readonly IProperty<bool> _samplingDayValid = H.Property<bool>();


        public string SamplingOrigin
        {
            get => _samplingOrigin.Get();
            set => _samplingOrigin.Set(value);
        }

        readonly IProperty<string> _samplingOrigin = H.Property<string>(c => c.Default(""));


        public string PharmacopoeiaVersion
        {
            get => _pharmacopoeiaVersion.Get();
            set => _pharmacopoeiaVersion.Set(value);
        }

        readonly IProperty<string> _pharmacopoeiaVersion = H.Property<string>(c => c.Default(""));


        public bool InOriginalPackaging
        {
            get => _inOriginalPackaging.Get();
            set => _inOriginalPackaging.Set(value);
        }

        readonly IProperty<bool> _inOriginalPackaging = H.Property<bool>();


        public string PrimaryPackaging
        {
            get => _primaryPackaging.Get();
            set => _primaryPackaging.Set(value);
        }

        readonly IProperty<string> _primaryPackaging = H.Property<string>(c => c.Default(""));


        public string SecondaryPackaging
        {
            get => _secondaryPackaging.Get();
            set => _secondaryPackaging.Set(value);
        }

        readonly IProperty<string> _secondaryPackaging = H.Property<string>(c => c.Default(""));


        public double? ReceivedQuantity
        {
            get => _receivedQuantity.Get();
            set => _receivedQuantity.Set(value);
        }

        readonly IProperty<double?> _receivedQuantity = H.Property<double?>();


        public string Aspect
        {
            get => _aspect.Get();
            set => _aspect.Set(value);
        }

        readonly IProperty<string> _aspect = H.Property<string>(c => c.Default(""));


        public string Size
        {
            get => _size.Get();
            set => _size.Set(value);
        }

        readonly IProperty<string> _size = H.Property<string>(c => c.Default(""));


        public bool HasInstruction
        {
            get => _hasInstruction.Get();
            set => _hasInstruction.Set(value);
        }

        readonly IProperty<bool> _hasInstruction = H.Property<bool>();


        public bool NoticeFr
        {
            get => _noticeFr.Get();
            set => _noticeFr.Set(value);
        }

        readonly IProperty<bool> _noticeFr = H.Property<bool>();


        public bool NoticeEn
        {
            get => _noticeEn.Get();
            set => _noticeEn.Set(value);
        }

        readonly IProperty<bool> _noticeEn = H.Property<bool>();


        public string InstructionLanguages
        {
            get => _instructionLanguages.Get();
            set => _instructionLanguages.Set(value);
        }

        readonly IProperty<string> _instructionLanguages = H.Property<string>(c => c.Default(""));


        public string StorageConditions
        {
            get => _storageConditions.Get();
            set => _storageConditions.Set(value);
        }

        readonly IProperty<string> _storageConditions = H.Property<string>(c => c.Default(""));


        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }

        readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));


        public string Conclusion
        {
            get => _conclusion.Get();
            set => _conclusion.Set(value);
        }

        readonly IProperty<string> _conclusion = H.Property<string>(c => c.Default(""));


         public DateTime? NotificationDate
        {
            get => _notificationDate.Get().ToUniversalTime();
            set => _notificationDate.Set(value.ToUniversalTime());
        }

         readonly IProperty<DateTime?> _notificationDate = H.Property<DateTime?>();


        public int? ValidatorId
        {
            get => _validator.Id.Get();
            set => _validator.Id.Set(value);
        }

        public User Validator
        {
            get => _validator.Get();
            set => ValidatorId = value?.Id;
        }

        readonly IForeign<User> _validator = H.Foreign<User>();


        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }

        readonly IProperty<double> _progress = H.Property<double>();


        public sbyte? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }

        readonly IProperty<sbyte?> _validation = H.Property<sbyte?>();


        public ConformityState ConformityId
        {
            get => _conformityId.Get();
            set => _conformityId.Set(value);
        }

        readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>();

        public string StageId
        {
            get => _stageId.Get();
            set => _stageId.Set(value);
        }

        readonly IProperty<string> _stageId = H.Property<string>();

        public string PreviousStageId
        {
            get => _previousStageId.Get();
            set => _previousStageId.Set(value);
        }

        readonly IProperty<string> _previousStageId = H.Property<string>();

        public SampleWorkflow.Stage Stage
        {
            get => _stage.Get();
            set => StageId = value.Name;
        }

        readonly IProperty<SampleWorkflow.Stage> _stage = H.Property<SampleWorkflow.Stage>(c => c
            .Set(e => SampleWorkflow.StageFromName(e.StageId))
            .On(e => e.StageId)
            .Update()
        );


        public int? CustomerId
        {
            get => _customer.Id.Get();
            set => _customer.Id.Set(value);
        }
        public Customer Customer
        {
            get => _customer.Get();
            set => CustomerId = value?.Id;
        }

        readonly IForeign<Customer> _customer = H.Foreign<Customer>();


        public int? ManufacturerId
        {
            get => _manufacturer.Id.Get();
            set => _manufacturer.Id.Set(value);
        }
        public virtual Manufacturer Manufacturer
        {
            get => _manufacturer.Get();
            set => _manufacturer.Set(value);
        }

        readonly IForeign<Manufacturer> _manufacturer = H.Foreign<Manufacturer>();


        public int? PharmacopoeiaId
        {
            get => _pharmacopoeia.Id.Get();
            set => _pharmacopoeia.Id.Set(value);
        }
        public Pharmacopoeia Pharmacopoeia
        {
            get => _pharmacopoeia.Get();
            set => _pharmacopoeia.Set(value);
        }

        readonly IForeign<Pharmacopoeia> _pharmacopoeia = H.Foreign<Pharmacopoeia>();


        public int? ProductId
        {
            get => _product.Id.Get();
            set => _product.Id.Set(value);
        }
        public Product Product
        {
            get => _product.Get();
            set => _product.Set(value);
        }

        readonly IForeign<Product> _product = H.Foreign<Product>( );

        public int? AnalysisMotivationId
        {
            get => _analysisMotivation.Id.Get();
            set => _analysisMotivation.Id.Set(value);
        }
        public AnalysisMotivation AnalysisMotivation
        {
            get => _analysisMotivation.Get();
            set => _analysisMotivation.Set(value);
        }

        readonly IForeign<AnalysisMotivation> _analysisMotivation = H.Foreign<AnalysisMotivation>( );

        public ObservableQuery<SampleTest> SampleTests => _sampleTests.Get();

        readonly IProperty<ObservableQuery<SampleTest>> _sampleTests = H.Property<ObservableQuery<SampleTest>>(c => c
            .Foreign(e => e.SampleId)
        );

        
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

        readonly IProperty<bool> _invoiced = H.Property<bool>();


        public bool Paid
        {
            get => _paid.Get();
            set => _paid.Set(value);
        }

        readonly IProperty<bool> _paid = H.Property<bool>();


        public string InvoiceNo
        {
            get => _invoiceNo.Get();
            set => _invoiceNo.Set(value);
        }

        readonly IProperty<string> _invoiceNo = H.Property<string>();

        public bool Expired => _expired.Get();

        readonly IProperty<bool> _expired = H.Property<bool>(c =>
            c.On(e => e.ExpirationDate).Set(e =>
            {
                if (e.ExpirationDate == null) return false;
                return DateTime.Now > e.ExpirationDate;
            }));

        public string ExpirationString => _expirationString.Get();

        readonly IProperty<string> _expirationString = H.Property<string>(c => c
            .On(e => e.ExpirationDate)
            .On(e => e.ExpirationDayValid)
            
            .Set(e =>
            {
                //Todo date localisation
                if (e.ExpirationDate == null) return "N/A";
                if (e.ExpirationDayValid) return e.ExpirationDate.Value.ToString("dd/MM/yyyy");
                return e.ExpirationDate.Value.ToString("MM/yyyy");
            }));

        public TimeSpan Life => _life.Get();

        readonly IProperty<TimeSpan> _life = H.Property<TimeSpan>(c => c
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

        public bool EndOfLife => _endOfLife.Get();

        readonly IProperty<bool> _endOfLife = H.Property<bool>(c => c
            .On(e => e.ExpirationDate)
            .NotNull(e => e.ExpirationDate)
            .Set(e =>
            {
                if (e.ExpirationDate == null) return false;
                return DateTime.Now > e.ExpirationDate.Value.Subtract(new TimeSpan(e.Life.Ticks / 3));
            }));

        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Reference)
            .Set(e => string.IsNullOrWhiteSpace(e.Reference)?"{New sample}":e.Reference)
        );

    }
}
