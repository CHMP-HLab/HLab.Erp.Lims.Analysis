using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<Form>;

    public partial class Form : Entity, IListableModel, ILocalCache
    {
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

        [Ignore]
        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .On(e => e.EnglishName)
            .Set(e => "Icons/Forms/" + e.EnglishName)
        );

        [Ignore]
        public string Caption => Name;
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => e.Name)
        );

    }
}
