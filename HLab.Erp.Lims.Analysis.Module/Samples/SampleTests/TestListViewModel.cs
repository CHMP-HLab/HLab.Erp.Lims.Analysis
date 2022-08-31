using System;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests;

public class TestsListViewModel : Core.EntityLists.EntityListViewModel<SampleTest>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    {
    }

    readonly IAclService _acl;

    public TestsListViewModel(IAclService acl, Injector i) : base(i, c => c

        .ColumnListable(e => e.Sample, "Sample")

        .ColumnListable(s => s.Sample.Product)

        .ColumnListable(e => e.Pharmacopoeia, "Pharmacopoeia")

        .ColumnListable(e => e.TestClass, "TestClass")

        .DescriptionColumn(t => "", t => t.Specification, "Specification")
        .Header("{Specifications}")
        .OrderBy(s => s.Specification).UpdateOn(s => s.Specification)

        .DescriptionColumn(t => "", t => t.Result.Result, "Result")
        .Header("{Result}")
        .OrderBy(s => s.Result?.Result ?? "").UpdateOn(s => s.Result.Result)

        .ConformityColumnPostLinked(s => s.Result == null ? ConformityState.NotChecked : s.Result.ConformityId)
            .UpdateOn(s => s.Result.ConformityId)

        .StageColumn(default(SampleTestWorkflow), s => s.StageId)

        .AddProperty("IsValid", s=>s?.Stage != null, s => s.Stage != SampleTestWorkflow.InvalidatedResults)
        .AddProperty("Group", s=>s?.TestClassId!=null, s => s.TestClassId)

    )
    {
        _acl = acl;
        var n = SampleTestWorkflow.Specifications; // this is a hack to force top level static constructor
    }

    protected override bool CanExecuteDelete(SampleTest sampleTest, Action<string> errorAction)
    {
        if (sampleTest == null) return false;
        var stage = sampleTest.Stage.IsAny(errorAction, SampleTestWorkflow.Specifications);
        var granted = _acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest);
        return stage && granted;
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}