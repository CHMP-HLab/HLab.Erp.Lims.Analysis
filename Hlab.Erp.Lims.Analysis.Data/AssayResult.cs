using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    [Table("TestResultat")]
    public partial class AssayResult : Entity<AssayResult>
        , IEntityWithIcon
        , IEntityWithColor
    {
        public int? Color => _color.Get();
        IProperty<int?> _color = H.Property<int?>(c => c.OneWayBind(e => e.SampleAssay.AssayClass.Color));

        public string IconName => _iconName.Get();
        IProperty<string> _iconName = H.Property<string>(c => c.OneWayBind(e => e.SampleAssay.AssayClass.IconName));

        [Column("TestEchantillonId")]
        public int? SampleAssayId
        {
            get => _sampleAssayId.Get();
            set => _sampleAssayId.Set(value);
        }
        IProperty<int?> _sampleAssayId = H.Property<int?>();

        [TriggerOn(nameof(SampleAssayId))]
        public virtual SampleAssay SampleAssay
        {
            //get => E.GetForeign<SampleAssay>(() => SampleAssayId); set => SampleAssayId = value.Id;
            get => _sampleAssay.Get();
            set => _sampleAssay.Set(value);
        }
        IProperty<SampleAssay> _sampleAssay = H.Property<SampleAssay>();

        [Column("UtilisateurId")]
        public int? UserId
        {
            get => _userId.Get();
            set => _userId.Set(value);
        }
        IProperty<int?> _userId = H.Property<int?>();

        [Column("Valeurs")]
        public string Values
        {
            get => _values.Get();
            set => _values.Set(value);
        }
        IProperty<string> _values = H.Property<string>(c => c.Default(""));

        [Column("Resultat")]
        public string Result
        {
            get => _results.Get(); set => _results.Set(value);
        }
        IProperty<string> _results = H.Property<string>(c => c.Default(""));

        //[Column("DateDebut")]
        //public DateTime? StartDate
        //{
        //    get => N.Get(() => (DateTime?)null); set => N.Set(value);
        //}

        //[Column("DateFin")]
        //public DateTime? EndDate
        //{
        //    get => N.Get(() => (DateTime?)null); set => N.Set(value);
        //}

        [Column("EtatId")]
        public int? StateId
        {
            get => _stateId.Get();
            set => _stateId.Set(value);
        }
        IProperty<int?> _stateId = H.Property<int?>();

    }
}
