using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<Pharmacopoeia>;

    public partial class Pharmacopoeia : Entity, IListableModel, ILocalCache
    {
        public Pharmacopoeia() => H.Initialize(this);

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
        private readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));

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
        private readonly IProperty<string> _searchUrl = H.Property<string>(c => c.Default(""));


        public string ReferenceUrl
        {
            get => _referenceUrl.Get();
            set => _referenceUrl.Set(value);
        }
        private readonly IProperty<string> _referenceUrl = H.Property<string>(c => c.Default(""));

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c.Default(""));

        [Ignore]
        public string Caption => _caption.Get();
        private readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => e.Name)
        );
        

        public static Pharmacopoeia DesignModel => new Pharmacopoeia
        {
            Name = "{US Pharmacopoeia}",
            Abbreviation = "Usp"
        };

    }
}
