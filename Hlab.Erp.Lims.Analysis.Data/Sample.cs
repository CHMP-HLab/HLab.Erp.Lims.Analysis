using System;
using HLab.Base.Extentions;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public partial class Sample : Entity<Sample>
    {

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

        public int? RecordYear
        {
            get => _recordYear.Get();
            set => _recordYear.Set(value);
        }
        private readonly IProperty<int?> _recordYear = H.Property<int?>();


        public int? RecordRequestId
        {
            get => _recordRequestId.Get();
            set => _recordRequestId.Set(value);
        }
        private readonly IProperty<int?> _recordRequestId = H.Property<int?>();

        [Ignore]
        public string RecordRequest
        {
            get => _recordRequest.Get();
            set
            {
                switch (value)
                {
                    case "EXT":
                        RecordRequestId = 0;
                        break;
                    case "LEM":
                        RecordRequestId = 1;
                        break;
                    case "PTS":
                        RecordRequestId = 2;
                        break;
                    case "REC":
                        RecordRequestId = 3;
                        break;
                    default:
                    throw new ArgumentException();
                }
            }
        }
        private readonly IProperty<string> _recordRequest = H.Property<string>(c => c
            .On(e => e.RecordRequestId)
            .Set(e => {
                switch (e.RecordRequestId)
                {
                    case 0: return "EXT";
                    case 1: return "LEM";
                    case 2: return "PTS";
                    case 3: return "REC";
                    case null: return "???";
                }
                throw new ArgumentException();
            })
        );

        public string RecordNo
        {
            get => _recordNo.Get();
            set => _recordNo.Set(value);
        }
        private readonly IProperty<string> _recordNo = H.Property<string>();

        [Ignore]
        public string Ref
        {
            get => _ref.Get();
            set
            {
                var parts = value.Split('/');
                RecordYear = int.Parse(parts[0]);
                RecordRequest = parts[1];
                RecordNo = parts[2];
            }
        }
        private readonly IProperty<string> _ref = H.Property<string>(c => c
            .On(e => e.RecordYear)
            .On(e => e.RecordRequest)
            .On(e => e.RecordNo)
            .Set(e => e.RecordYear + "/" + e.RecordRequest + "/" + ("0000" + e.RecordNo).Right(4))
        );


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

        [System.ComponentModel.DataAnnotations.Schema.Column]
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


        [System.ComponentModel.DataAnnotations.Schema.Column(/*TypeName = "tinyint(1)"*/)]
        public bool HasInstruction
        {
            get => _hasInstruction.Get();
            set => _hasInstruction.Set(value);
        }
        private readonly IProperty<bool> _hasInstruction = H.Property<bool>();


        [System.ComponentModel.DataAnnotations.Schema.Column("NoticeFrancaise"/*, TypeName = "tinyint(1)"*/)]
        public bool NoticeFr
        {
            get => _noticeFr.Get();
            set => _noticeFr.Set(value);
        }
        private readonly IProperty<bool> _noticeFr = H.Property<bool>();


        [System.ComponentModel.DataAnnotations.Schema.Column("NoticeAnglaise"/*, TypeName = "tinyint(1)"*/)]
        public bool NoticeEn
        {
            get => _noticeEn.Get();
            set => _noticeEn.Set(value);
        }
        private readonly IProperty<bool> _noticeEn = H.Property<bool>();


        [System.ComponentModel.DataAnnotations.Schema.Column()]
        public string InstructionLanguages
        {
            get => _instructionLanguages.Get();
            set => _instructionLanguages.Set(value);
        }
        private readonly IProperty<string> _instructionLanguages = H.Property<string>(c => c.Default(""));


        [System.ComponentModel.DataAnnotations.Schema.Column]
        public string StorageConditions
        {
            get => _storageConditions.Get();
            set => _storageConditions.Set(value);
        }
        private readonly IProperty<string> _storageConditions = H.Property<string>(c => c.Default(""));


        [System.ComponentModel.DataAnnotations.Schema.Column]
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));


        [System.ComponentModel.DataAnnotations.Schema.Column]
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
            get => _validatorId.Get();
            set => _validatorId.Set(value);
        }
        private readonly IProperty<int?> _validatorId = H.Property<int?>();


        public string Validator
        {
            get => _validator.Get();
            set => _validator.Set(value);
        }
        private readonly IProperty<string> _validator = H.Property<string>(c => c.Default(""));


        public sbyte? Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }
        private readonly IProperty<sbyte?> _progress = H.Property<sbyte?>();


        [System.ComponentModel.DataAnnotations.Schema.Column]
        public sbyte? Validation
        {
            get => _validation.Get();
            set => _validation.Set(value);
        }
        private readonly IProperty<sbyte?> _validation = H.Property<sbyte?>();


        public int? State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private readonly IProperty<int?> _state = H.Property<int?>();

        public string Stage
        {
            get => _stage.Get();
            set => _stage.Set(value);
        }
        private readonly IProperty<string> _stage = H.Property<string>();

        [System.ComponentModel.DataAnnotations.Schema.Column]
        public sbyte? EtatPC
        {
            get => _etatPC.Get();
            set => _etatPC.Set(value);
        }
        private readonly IProperty<sbyte?> _etatPC = H.Property<sbyte?>();


        [System.ComponentModel.DataAnnotations.Schema.Column]
        public sbyte? EtatMB
        {
            get => _etatMB.Get();
            set => _etatMB.Set(value);
        }
        private readonly IProperty<sbyte?> _etatMB = H.Property<sbyte?>();


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
            get => _manufacturerId.Get();
            set => _manufacturerId.Set(value);
        }
        private readonly IProperty<int?> _manufacturerId = H.Property<int?>();

        [Ignore]
        public virtual Manufacturer Manufacturer
        {
            get => _manufacturer.Get(); // E.GetForeign<Manufacturer>(() => ManufacturerId);
            set => ManufacturerId = value.Id;
            //set => _manufacturer.Set(Context.Db.GetOrAdd(value), () => ManufacturerId = value.Id);
        }
        private readonly IProperty<Manufacturer> _manufacturer = H.Property<Manufacturer>(c => c
            .Foreign(e => e.ManufacturerId)
        );


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

        [Ignore] public ObservableQuery<SampleTest> SampleTests => _sampleTests.Get();
        //{
        //    get => _sampleTests.Get();
        //}

        private readonly IProperty<ObservableQuery<SampleTest>> _sampleTests = H.Property<ObservableQuery<SampleTest>>(c => c
            .Foreign(e => e.SampleId)
        );

        [Ignore]
        public static Sample DesignModel => new Sample
        {
                RecordYear = 2019,
                RecordNo = "42",
                ReceivedQuantity = 100,
                ReceptionDate = DateTime.Now,
                Progress = 50,

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

        //        public virtual Utilisateur Utilisateur { get; set; }
        //        public virtual Utilisateur UtilisateurValidateur { get; set; }, Func<INotifyPropertyChanged, INotifier> n) : base(ctx,n)
        //public Sample(ErpContext ctx) : base(ctx)
        //{
        //}
        //public Sample() : base(null)
        //{
        //}
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

    }
}
