using HLab.Erp.Data;
using HLab.Notify;
using HLab.Notify.Extentions;
using HLab.Notify.Triggers;
using NPoco;

namespace HLab.Erp.Lims.Analisis.Data
{
    [TableName("Produit")]
     public partial class Product : Entity<Product>
     {
        [Column("Dci")]
        public string Inn
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Dosage")]
        public string Dose
        {
            get => N.Get(() => ""); set => N.Set(value);
        }


        [Column("Remarque")]
        public string Note
        {
            get => N.Get(() => ""); set => N.Set(value);
        }

         [TriggedOn(nameof(Inn))]
         [TriggedOn(nameof(Dose))]
         public string Caption => this.Get(() => Inn + " (" + Dose + ")");

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


        [Column("FormeId")]
        public int? FormId
        {
            get => N.Get(() => (int?)null); set => N.Set(value);
        }

        [Ignore][TriggedOn(nameof(FormId))]
        public Form Form
        {
            get => this.DbGetForeign<Form>(()=>FormId); set => FormId = value.Id;
        }

    }
}
