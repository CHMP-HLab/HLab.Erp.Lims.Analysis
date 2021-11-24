using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Manufacturer>;

    public partial class Manufacturer : Corporation, ILocalCache, IListableModel
    {
        public Manufacturer() => H.Initialize(this);

        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New manufacturer}":e.Name)
            .On(e => e.Name)
            .Update()
        );

        public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .Set(e => e.Country?.IconPath)
            .On(e => e.Country.IconPath)
            .Update()
        );

    }
}
