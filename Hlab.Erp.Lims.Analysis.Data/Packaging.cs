using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Packaging>;

    public class Packaging : Entity, ILocalCache
    {
        public Packaging() => H.Initialize(this);

        public bool Secondary
        {
            get => _secondary.Get();
            set => _secondary.Set(value);
        }
        private readonly IProperty<bool> _secondary = H.Property<bool>();


        [Column("Nom")]
        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>();

    }
}