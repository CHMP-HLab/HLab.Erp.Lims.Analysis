using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data
{
    public class Packaging : Entity<Packaging>, ILocalCache
    {
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