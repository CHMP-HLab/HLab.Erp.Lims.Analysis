using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Extentions;
using HLab.Notify.Triggers;
using NPoco;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("Forme")]
    public partial class Form : Entity<Form>, IListableModel
    {
        public override string ToString() => Name;

        [Column("Nom")]
        public string Name
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [Column("NomUs")]
        public string EnglishName
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

        [TriggedOn(nameof(EnglishName))]
        public string IconName => this.Get(() => "Forms/" + EnglishName);

        [TriggedOn(nameof(Name))]
        public string Caption => Name;

    }
}
