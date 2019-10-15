using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    [Table("Pharmacopee")]
    public partial class Pharmacopoeia : Entity<Pharmacopoeia>, IListableModel, ILocalCache
    {
        public string NameFr
        {
            get => _nameFr.Get();
            set => _nameFr.Set(value);
        }
        private IProperty<string> _nameFr = H.Property<string>(c => c.Default(""));

        public string NameEn
        {
            get => _nameEn.Get();
            set => _nameEn.Set(value);
        }
        private readonly IProperty<string> _nameEn = H.Property<string>(c => c.Default(""));

        public string Abbreviation
        {
            get => _abbreviation.Get();
            set => _abbreviation.Set(value);
        }
        private readonly IProperty<string> _abbreviation = H.Property<string>(c => c.Default(""));


        public string Url
        {
            get => _url.Get();
            set => _url.Set(value);
        }
        private readonly IProperty<string> _url = H.Property<string>(c => c.Default(""));


        public string SearchUrl
        {
            get => _searchUrl.Get();
            set => _searchUrl.Set(value);
        }
        private IProperty<string> _searchUrl = H.Property<string>(c => c.Default(""));


        public string ReferenceUrl
        {
            get => _referenceUrl.Get();
            set => _referenceUrl.Set(value);
        }
        private readonly IProperty<string> _referenceUrl = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.NameFr)
            .Set(e => e.NameFr)
        );
        

        [Ignore]
        public string IconName => _iconName.Get();
        private readonly IProperty<string> _iconName = H.Property<string>(c => c
            .On(e => e.NameFr)
            .Set(e => "Pharmacopoeia/" + (string.IsNullOrWhiteSpace(e.Abbreviation)?"home_flag":e.Abbreviation))
        );

    }
}
