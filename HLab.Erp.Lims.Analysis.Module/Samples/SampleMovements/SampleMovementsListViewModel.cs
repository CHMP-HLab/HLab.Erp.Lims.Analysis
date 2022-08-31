using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleMovements;

public class SampleMovementsListViewModel : EntityListViewModel<SampleMovement>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }

    protected override bool CanExecuteAdd(Action<string> errorAction) => true;
    protected override bool CanExecuteDelete(SampleMovement target,Action<string> errorAction) => Selected!=null || (SelectedIds?.Any()??false);


    readonly Sample _sample;

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
        _sample = sample;
    }


    readonly SampleTestResult _result;

    public SampleMovementsListViewModel(SampleTestResult result, Injector i) : base(i, c => c
        .StaticFilter(e => e.SampleTestResultId == result.Id)

        .Column("Motivation")
        .Header("{Motivation}")
        .Width(80)
        .Localize(s => s.Motivation.Caption).Icon(s => s.Motivation.IconPath)
        .Link(s => s.Motivation)
        .Filter()
        .IconPath("Icons/Entities/SampleMovementMotivation")

        .Column("Quantity").Content(e => e.Quantity)
        .Header("{Quantity}")
    )
    {
        _sample = result.SampleTest.Sample;
        _result = result;
    }

    public override Type AddArgumentClass => typeof(SampleMovementMotivation);

    protected override Task ConfigureNewEntityAsync(SampleMovement sm, object arg)
    {
        if (arg is not SampleMovementMotivation motivation) return Task.CompletedTask;

        sm.Sample = _sample;
        sm.SampleTestResult = _result;
        sm.Motivation = motivation;
        sm.Quantity = 1;

        return Task.CompletedTask;
    }


    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}