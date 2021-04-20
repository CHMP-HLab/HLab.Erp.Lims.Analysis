using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Manufacturer>;

    public partial class Manufacturer : Corporation, ILocalCache, IListableModel
    {
        public Manufacturer() => H.Initialize(this);

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .On(e => e.Id)
            //TODO : localize
            .Set(e => (e.Id < 0 && string.IsNullOrEmpty(e.Name)) ? "Nouveau client" : e.Name)
        );

        [Ignore] public string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
            .Bind(e => e.Country.IconPath)
        );

    }
}
