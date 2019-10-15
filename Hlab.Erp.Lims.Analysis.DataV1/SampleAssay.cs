using System;
using HLab.Erp.Data;
using HLab.Notify.Triggers;
using NPoco;
using HLab.Notify;
using HLab.Notify.Extentions;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("TestEchantillon")]
    public partial class SampleAssay : Entity<SampleAssay>
        , IEntityWithIcon
        , IEntityWithColor
    {
        [Column("EchantillonId")]
        public int? SampleId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("TestTypeId")]
        public int? AssayClassId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("TestEtatId")]
        public int? AssayStateId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("UtilisateurId")]
        public int? UserId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("Methode")]
        public string Method { get; set; }

        [Column("MotivationId")]
        public int? PurposeId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("Commentaire")]
        public string Comment { get; set; }

        [Column]
        public int? Validation
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column]
        public int? ValidateurId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column]
        public DateTime? DateValidation
        {
            get => N.Get(() => (DateTime?)null); set => N.Set(value);
        }


        [Column("NomTest")]
        public string AssayName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Version
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        //[StringLength(16777215)]
        //public string Cs1 { get; set; }

        //[StringLength(16777215)]
        //public string Xaml1 { get; set; }

        [Column]
        public byte[] Code
        {
            get => N.Get(() => (byte[])null); set => N.Set(value);
        }


        [Column("Valeurs")]
        public string Values
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Description
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Resultat")]
        public string Result
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Norme")]
        public string Specification
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column("Conforme")]
        public string Conform
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("DateDebut")]
        public DateTime? StartDate
        {
            get => N.Get(() => (DateTime?)null); set => N.Set(value);
        }


        [Column("DateFin")]
        public DateTime? EndDate
        {
            get => N.Get(() => (DateTime?)null); set => N.Set(value);
        }


        [Column]
        public bool? ReTest
        {
            get => N.Get(() => (bool?)null); set => N.Set(value);
        }


        [Column]
        public string NumeroOos
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public int? Ordre
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [TriggedOn(nameof(SampleId))]
        public virtual Sample Sample
        {
            get => this.DbGetForeign<Sample>(() => SampleId); set => SampleId = value.Id;
        }


        [TriggedOn(nameof(AssayClassId))]
        public virtual AssayClass AssayClass
        {
            get => this.DbGetForeign<AssayClass>(() => AssayClassId); set => AssayClassId = value.Id;
        }

        [TriggedOn(nameof(AssayClass), "Color")]
        public int? Color => N.Get(() => AssayClass.Color);


        [TriggedOn(nameof(AssayClass), "IconName")]
        public string IconName => N.Get(() => AssayClass.IconName);
    }
}
