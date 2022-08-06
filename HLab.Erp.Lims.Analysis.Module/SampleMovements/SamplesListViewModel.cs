using System;
using System.Linq;
using System.Windows.Input;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleMovements;

public class SampleMovementMotivationsList : EntityListViewModel<SampleMovementMotivation>
{
    public SampleMovementMotivationsList(Injector injector) : base(injector, c => c
        .Column("Motivation")
        .Header("{Motivation}")
        .Link(s => s.Name)
        .Filter()
    )
    {
    }
}

public class SampleMovementsListViewModel : EntityListViewModel<SampleMovement>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }

    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(SampleMovement target,Action<string> errorAction) => Selected!=null || (SelectedIds?.Any()??false);

    public override ICommand AddCommand { get; }


    public SampleMovementsListViewModel(Sample sample, Injector i) : base(i, c => c
        .StaticFilter(e => e.SampleId == sample.Id)

        .Column("Motivation")
        .Header("{Motivation}")
        .Width(80)
        .Link(s => s.Motivation)
        .Filter()
        .IconPath("Icons/Entities/SampleMovementMotivation")

        .Column("Quantity")
        .Header("{Quantity}").Content(e => e.Quantity)
    )
    {
    }

    public SampleMovementsListViewModel(SampleTestResult result, Injector i) : base(i, c => c
        .StaticFilter(e => e.SampleTestResultId == result.Id)

        .Column("Motivation")
        .Header("{Motivation}")
        .Width(80)
        .Link(s => s.Motivation)
        .Filter()
        .IconPath("Icons/Entities/SampleMovementMotivation")

        .Column("Quantity")
        .Header("{Quantity}")
    )
    {
    }

    protected override void ConfigureEntity(SampleMovement movement)
    {
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}