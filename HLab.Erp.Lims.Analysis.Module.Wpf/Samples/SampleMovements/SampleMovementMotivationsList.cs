using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data.Entities;

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