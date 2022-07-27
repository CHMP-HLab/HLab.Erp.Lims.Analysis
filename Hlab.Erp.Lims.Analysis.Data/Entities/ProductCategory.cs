using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Entities
{
    using H = HD<ProductCategory>;

    public class ProductCategory : Entity, ILocalCache, IListableModel
    {
        public static TestCategory DesignModel => new TestCategory
        {
            Name = "Design Category",
            Priority = 1,
            IconPath = "Icons/Default"
        };

        public ProductCategory() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string NamePropertyName
        {
            get => _namePropertyName.Get();
            set => _namePropertyName.Set(value);
        }

        readonly IProperty<string> _namePropertyName = H.Property<string>(c => c.Default("{Name}"));

        public string VariantPropertyName
        {
            get => _variantPropertyName.Get();
            set => _variantPropertyName.Set(value);
        }

        readonly IProperty<string> _variantPropertyName = H.Property<string>(c => c.Default("{Variant}"));

        public string ComplementPropertyName
        {
            get => _complementPropertyName.Get();
            set => _complementPropertyName.Set(value);
        }

        readonly IProperty<string> _complementPropertyName = H.Property<string>(c => c.Default("{Complement}"));

        public int? Priority
        {
            get => _priority.Get();
            set => _priority.Set(value);
        }

        readonly IProperty<int?> _priority = H.Property<int?>();

        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New product category}":e.Name)
        );
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));
    }

}