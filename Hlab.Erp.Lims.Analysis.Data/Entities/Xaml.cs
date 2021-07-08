using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<Xaml>;

    public class Xaml : Entity, ILocalCache
    {
        public Xaml() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));
        public string Page
        {
            get => _page.Get();
            set => _page.Set(value);
        }

        private readonly IProperty<string> _page = H.Property<string>(c => c.Default(""));
    }
}
