using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = H<AnalysisMotivation>;

    public partial class AnalysisMotivation : Entity, IListableModel, ILocalCache
    {
        public static AnalysisMotivation DesignModel => new() { Name="My Form"};

        public AnalysisMotivation() => H.Initialize(this);

        public override string ToString() => Name;


        public string Name
        {
            get => _name.Get(); set => _name.Set(value);
        }

        readonly IProperty<string> _name = H.Property<string>(c => c.Default(""));


        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }

        readonly IProperty<string> _iconPath = H.Property<string>();

        [Ignore]
        public string Caption => _caption.Get();

        readonly IProperty<string> _caption = H.Property<string>(c => c
            .On(e => e.Name)
            .Set(e => string.IsNullOrWhiteSpace(e.Name)?"{New motivation}":$"{{Motivation}}\n{e.Name}")
        );

    }
}
