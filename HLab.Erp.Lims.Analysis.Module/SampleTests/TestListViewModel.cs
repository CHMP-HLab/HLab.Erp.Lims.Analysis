using System;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class TestsListViewModel : Core.EntityLists.EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
        }

        public TestsListViewModel(Injector i) : base(i, c => c

                   .Column(e => e.Sample,"Sample")

                   //.Column(s => s.Sample.Product)

                   .Column(e => e.Pharmacopoeia,"Pharmacopoeia").Mvvm()

                   .Column(e => e.TestClass,"TestClass").Mvvm()

                .DescriptionColumn(t => "", t => t.Specification,"Specification")
                    .Header("{Specifications}")
                    .OrderBy(s => s.Specification).UpdateOn(s => s.Specification)

                .DescriptionColumn(t => "", t => t.Result?.Result,"Result")
                    .Header("{Result}")
                    .OrderBy(s => s.Result?.Result ?? "").UpdateOn(s => s.Result.Result)

                .ConformityColumnPostLinked(s => s.Result == null ? ConformityState.NotChecked : s.Result.ConformityId).UpdateOn(s => s.Result.ConformityId)

                .StageColumn(default(SampleTestWorkflow), s => s.StageId)

                .Column().Hidden().Header("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults)
                .Column().Hidden().Header("Group").Content(s => s.TestClassId)

        )
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

        }

        protected override bool CanExecuteDelete(SampleTest sampleTest,Action<string> errorAction)
        {
            if (sampleTest == null) return false;
            var stage = sampleTest.Stage.IsAny(errorAction, SampleTestWorkflow.Specifications);
            var granted = Injected.Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest);
            return stage && granted;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}