using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Product>;

    public partial class Product : Entity, ILocalCache, IListableModel
    {
        public Product() => H.Initialize(this);

        public string Complement
        {
            get => _complement.Get();
            set => _complement.Set(value);
        }
        private readonly IProperty<string> _complement = H.Property<string>(c => c.Default(""));

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
        .Set(e => e.Form?.IconPath)
            .On(e => e.Form.IconPath).Update()
        );


        public int? FormId
        {
            get => _form.Id.Get();
            set => _form.Id.Set(value);
        }

        [Ignore]
        public Form Form
        {
            set => _form.Set(value);
            get => _form.Get();
        }
        private readonly IForeign<Form> _form = H.Foreign<Form>();

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
