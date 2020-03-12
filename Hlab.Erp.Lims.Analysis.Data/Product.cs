using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
     public partial class Product : Entity<Product>, ILocalCache, IListableModel
     {
        public string Inn
        {
            get => _inn.Get();
            set => _inn.Set(value);
        }
        private readonly IProperty<string> _inn = H.Property<string>(c => c.Default(""));


        public string Dose
        {
            get => _dose.Get();
            set => _dose.Set(value);
        }
        private readonly IProperty<string> _dose = H.Property<string>(c => c.Default(""));


        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));
        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Inn)
            .On(e => e.Dose)
            .On(e => e.Form)
            .Set(e => e.Inn + " - " + (e.Form?.Caption??"") +  " (" + e.Dose + ")")
        );

        [Ignore]
        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .OneWayBind(e => e.Form.IconPath)
        );


        public int? FormId
        {
            get => _formId.Get();
            set => _formId.Set(value);
        }
        private readonly IProperty<int?> _formId = H.Property<int?>();

        [Ignore]
        public Form Form
        {
            set => FormId = value.Id;
            get => _form.Get();
        }
        private readonly IProperty<Form> _form = H.Property<Form>(c => c
            .Foreign(e => e.FormId)
        );
        public int? CategoryId
        {
            get => _category.Id.Get();
            set => _category.Id.Set(value);
        }
        [Ignore]
        public ProductCategory Category
        {
            get => _category.Get();
            set => _category.Set(value);
        }
        private readonly IForeign<ProductCategory> _category = H.Foreign<ProductCategory>();


        public static Product DesignModel => new Product
        {
            Inn = "Paracetamol",Dose ="20 mg",Note = "Design time model"
        };
     }
}
