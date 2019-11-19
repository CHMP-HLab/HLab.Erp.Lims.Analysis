using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public partial class Manufacturer : Corporation<Manufacturer>, ILocalCache, IListableModel
    {

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
            .OneWayBind(e => e.Country.IconPath)
        );

    }
}
