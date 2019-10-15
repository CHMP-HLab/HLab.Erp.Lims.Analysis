using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public partial class Form : Entity<Form>, IListableModel, ILocalCache
    {
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

        [Ignore][TriggerOn(nameof(EnglishName))]
        public string IconName => "Icons/Forms/" + EnglishName;

        [Ignore][TriggerOn(nameof(Name))]
        public string Caption => Name;

    }
}
