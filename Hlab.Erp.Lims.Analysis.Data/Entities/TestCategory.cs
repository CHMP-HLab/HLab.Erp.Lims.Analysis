using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<TestCategory>;

    public class TestCategory : Entity, ILocalCache, IListableModel
    {
        public TestCategory() => H.Initialize(this);

        public static TestCategory DesignModel => new TestCategory
        {
            Name = "Design Category",
            Priority = 1,
            IconPath = "Icons/Default"
        };

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public int? Priority
        {
            get => _priority.Get();
            set => _priority.Set(value);
        }

        readonly IProperty<int?> _priority = H.Property<int?>();
        [Ignore]
        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New test category}":$"{{Test category}}\n{e.Name}")
        );
        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));
    }

}