using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public partial class Manufacturer : Entity<Manufacturer>, ILocalCache
    {
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string Address
        {
            get => _address.Get();
            set => _address.Set(value);
        }
        private readonly IProperty<string> _address = H.Property<string>(c => c.Default(""));

        public int? CountryId
        {
            get => _countryId.Get();
            set => _countryId.Set(value);
        }
        private readonly IProperty<int?> _countryId = H.Property<int?>();

        [Ignore]
        public Country Country
        {
            get => _country.Get();
            //set => _country.Set(value);
            set => CountryId = value.Id;
        }
        private readonly IProperty<Country> _country = H.Property<Country>(c => c
            .Foreign(e => e.CountryId));


        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>(c => c.Default(""));

        public string RchEx
        {
            get => _rchEx.Get();
            set => _rchEx.Set(value);
        }
        private readonly IProperty<string> _rchEx = H.Property<string>(c => c.Default(""));

        public string RchAp
        {
            get => _rchAp.Get();
            set => _rchAp.Set(value);
        }
        private readonly IProperty<string> _rchAp = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Caption => Name;
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => e.Name)
        );
    }
}
