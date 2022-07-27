using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities
{
    using H = HD<Inn>;
    public class Inn : Entity, IListableModel, ILocalCache
    {
        public Inn() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

        public string CasNumber
        {
            get => _casNumber.Get();
            set => _casNumber.Set(value);
        }

        readonly IProperty<string> _casNumber = H.Property<string>(c => c.Default(""));

        public string UnitGroup
        {
            get => _unitGroup.Get(); 
            set => _unitGroup.Set(value);
        }

        readonly IProperty<string> _unitGroup = H.Property<string>(c => c.Default(""));

        public double? MolarMass
        {
            get => _molarMass.Get(); 
            set => _molarMass.Set(value);
        }

        readonly IProperty<double?> _molarMass = H.Property<double?>(c => c.Default((double?)default));

        public double? Density
        {
            get => _density.Get(); 
            set => _density.Set(value);
        }

        readonly IProperty<double?> _density = H.Property<double?>(c => c.Default((double?)default));

        [Ignore]
        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name) ? "{New INN}" : $"{e.Name}")
        );

        [Ignore]
        public string IconPath => "IconMolecule";

        public static Inn DesignModel => new (){Name = "Paracetamol"};
    }
}
