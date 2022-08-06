using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities;

using H = HD<SampleMovementMotivation>;

public class SampleMovementMotivation : Entity, IListableModel
{
    public SampleMovementMotivation()
    {
        H.Initialize(this);
    }

    public string Name
    {
        get => _name.Get();
        set => _name.Set(value);
    }
    readonly IProperty<string> _name = H.Property<string>();

    public string IconPath
    {
        get => _iconPath.Get();
        set => _iconPath.Set(value);
    }
    readonly IProperty<string> _iconPath = H.Property<string>();

    [Ignore] public string Caption => Name;

}