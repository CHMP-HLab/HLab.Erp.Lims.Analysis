using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Extentions;
using HLab.Notify.Triggers;
using NPoco;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("Pharmacopee")]
    public partial class Pharmacopoeia : Entity<Pharmacopoeia>, IListableModel
    {
        [Column("NomFrancais")]
        public string FrenchName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("NomAnglais")]
        public string EnglishName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        //Todo : coquille abrev'i'ation
        [Column("Abrevation")]
        public string Abbreviation
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string Url
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string SearchUrl
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column]
        public string ReferenceUrl
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [TriggedOn(nameof(FrenchName))]
        public string Caption => FrenchName;


        [TriggedOn(nameof(Abbreviation))]
        public string IconName => this.Get(() => "Pharmacopoeia/" + (string.IsNullOrWhiteSpace(Abbreviation)?"home_flag":Abbreviation));

    }
}
