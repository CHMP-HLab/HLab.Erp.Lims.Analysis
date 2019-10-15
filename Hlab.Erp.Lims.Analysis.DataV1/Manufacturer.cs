using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Notify.Triggers;
using NPoco;
using HLab.Notify;
using HLab.Notify.Extentions;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("Fabricant")]
    public partial class Manufacturer : Entity<Manufacturer>
    {
        [Column("Nom")]
        public string Name
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column("Adresse")]
        public string Adress
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column("PaysId")]
        public int? CountryId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Ignore]
        [TriggedOn(nameof(CountryId))]
        public Country Country
        {
            get => this.DbGetForeign<Country>(() => CountryId); set => CountryId = value.Id;
        }


        [Column("Remarque")]
        public string Note
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column]
        public string RchEx
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column]
        public string RchAp
        {
            get => N.Get(() => ""); set => N.Set(value);
        }
    }
}
