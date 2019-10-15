using System;
using System.Collections.Generic;
using HLab.Base;
using HLab.Base.Extentions;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Extentions;
using HLab.Notify.Triggers;
using NPoco;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("Echantillon")]
    public partial class Sample : Entity<Sample>
    {

        [Column("DossierId")]
        public int? FileId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Column("EnregistrementAnnee")]
//        [Column("Annee")]
        public int? RecordYear
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("EnregistrementDemande")]
//        [Column("Demande")]
        public int? RecordRequestId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [TriggedOn(nameof(RecordRequestId))]
        public string RecordRequest
        {
            get
            {
                switch (RecordRequestId)
                {
                    case 0: return "EXT";
                    case 1: return "LEM";
                    case 2: return "PTS";
                    case 3: return "REC";
                }
                throw new ArgumentException();
            }
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
                }
                throw new ArgumentException();

            }
        }

        [Column("EnregistrementNumero")]
//        [Column("Numero")]
        public int? RecordNo
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [TriggedOn(nameof(RecordYear))]
        [TriggedOn(nameof(RecordRequest))]
        [TriggedOn(nameof(RecordNo))]
        public string Ref
        {
            get => this.Get(() => RecordYear + "/" + RecordRequest + "/" + ("0000" + RecordNo).Right(4));
            set
            {
                var parts = value.Split('/');
                RecordYear = int.Parse(parts[0]);
                RecordRequest = parts[1];
                RecordNo = int.Parse(parts[2]);
            }
        }


        [Column("FeuilleTravail")]
        public string Worksheet

        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Specialite")]
        public string CommercialName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column("Lot")]
        public string Batch
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        //[Column(TypeName = "date")]
        //public DateTime? DateFabrication { get; set; }

        [Column("DatePeremption")]
        public DateTime? ExpiryDate 
        {
            get => N.Get(() => (DateTime?)null); set => N.Set(value);
        }

    //        public DateTime? DatePrelevement { get; set; }

    [Column("Prelevement")]
        public string Taking { get; set; }


        [Column("VersionPharmacopee")]
        public string PharmacopoeiaVersion
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("ConditionnementOrigine")]
        public bool? OriginalPackaging
        {
            get => N.Get(() => (bool?)null); set => N.Set(value);
        }


        [Column("ConditionnementPrimaire")]
        public string PrimairyPackaging
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("ConditionnementSecondaire")]
        public string SecondaryPackaging
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("QuantiteRecue")]
        public double? ReceivedQuantity
        {
            get => N.Get(() => (double?)null); set => N.Set(value);
        }

        [Column]
        public string Aspect
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Dimension")]
        public string Size
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public bool? Notice
        {
            get => N.Get(() => (bool?)null); set => N.Set(value);
        }


        [Column("NoticeFrancaise")]
        public bool? NoticeFr
        {
            get => N.Get(() => (bool?)null); set => N.Set(value);
        }


        [Column("NoticeAnglaise")]
        public bool? NoticeEn
        {
            get => N.Get(() => (bool?)null); set => N.Set(value);
        }


        [Column]
        public string NoticeAutre
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Stockage")]
        public string Storage
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Remarque")]
        public string Note
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Conclusion
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("DateNotification")]
        public DateTime? NotificationDate
        {
            get => N.Get(() => (DateTime?)null); set => N.Set(value);
        }


        [Column("ValidateurId")]
        public int? ValidatorId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("Validateur")]
        public string Validator
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Avancement")]
        public sbyte? Progress
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }


        [Column]
        public sbyte? Validation
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }


        [Column("Etat")]
        public sbyte? State
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }


        [Column]
        public sbyte? EtatPC
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }


        [Column]
        public sbyte? EtatMB
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }


        [Column("ClientId")]
        public int? CustomerId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        
        [Ignore][TriggedOn(nameof(CustomerId))]
        public Customer Customer
        {
            get => this.DbGetForeign<Customer>(() => CustomerId); set => CustomerId = value.Id;
        }



        [Column("FabricantId")]
        public int? ManufacturerId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        
        [Ignore][TriggedOn(nameof(ManufacturerId))]
        public virtual Manufacturer Manufacturer
        {
            get => this.DbGetForeign<Manufacturer>(() => ManufacturerId); set => ManufacturerId = value.Id;
        }


        [Column("PharmacopeeId")]
        public int? PharmacopoeiaId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        
        [Ignore][TriggedOn(nameof(PharmacopoeiaId))]
        public Pharmacopoeia Pharmacopoeia
        {
            get => this.DbGetForeign<Pharmacopoeia>(() => PharmacopoeiaId); set => PharmacopoeiaId = value.Id;
        }


        [Column("ProduitId")]
        public int? ProductId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        
        [Ignore][TriggedOn(nameof(ProductId))]
        public virtual Product Product
        {
            get => this.DbGetForeign<Product>(() => ProductId); set => ProductId = value.Id;
        }

        [Ignore]
        public virtual ICollection<SampleAssay> SampleAssays { get; set; }


//        public virtual Utilisateur Utilisateur { get; set; }
//        public virtual Utilisateur UtilisateurValidateur { get; set; }
    }
}
