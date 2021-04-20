using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<Form>;

    public partial class Form : Entity, IListableModel, ILocalCache
    {
        public static Form DesignModel => new() { Name="Tablet"};

        public Form() => H.Initialize(this);

        public override string ToString() => Name;


        public string Name
        {
            get => _name.Get(); set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string EnglishName
        {
            get => _englishName.Get();
            set => _englishName.Set(value);
        }
        private readonly IProperty<string> _englishName = H.Property<string>(c => c.Default(""));

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>();

        [Ignore]
        public string Caption => Name;
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => e.Name)
        );

    }
}
