using System.Collections.Generic;
using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Extentions;
using NPoco;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("TestType")]
    public partial class AssayClass : Entity<AssayClass>
        , IEntityWithIcon
        , IEntityWithColor
    {
        [Column("Nom")]
        public string Name
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Version
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Cs1
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Xaml1
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public byte[] Code
        {
            get => N.Get(() => (byte[])null); set => N.Set(value);
        }


        [Column("Ordre")]
        public int? Order
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }


        [Column("Categorie")]
        public sbyte? Category
        {
            get => N.Get(() => (sbyte?)null); set => N.Set(value);
        }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SampleAssay> SampleAssays { get; set; }

        [Column("DureePremier")]
        public int? DurationFirst
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Column("DureeSuivants")]
        public int? DurationNext
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Column("DureeAdmin")]
        public int? DurationAdmin
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Column]
        public int? Color
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Column("Icon")]
        public string IconName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }
    }
}
