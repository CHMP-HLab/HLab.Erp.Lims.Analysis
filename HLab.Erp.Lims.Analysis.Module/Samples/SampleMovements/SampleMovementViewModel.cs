using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;
using HLab.Erp.Lims.Analysis.Data.Entities;

namespace HLab.Erp.Lims.Analysis.Module.SampleMovements;

using H = H<SampleMovementViewModel>;

internal class SampleMovementViewModel: EntityViewModel<SampleMovement>
{
    internal class Design : SampleMovementViewModel, IViewModelDesign
    {
        public Design() : base(null)
        {
        }

        public new SampleMovement Model { get; } = SampleMovement.DesignModel;
    }

    public override string IconPath => _iconPath.Get();

    readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.Motivation.IconPath)
        .Update()
    );

    string GetIconPath => Model?.Motivation?.IconPath??base.IconPath;

    public SampleMovementViewModel(Injector i):base(i)
    {
        H.Initialize(this);
    }

}