using System;
using System.Windows;
using System.Windows.Controls;
using Google.Protobuf.Reflection;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Filters;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using Outils;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class TestsListViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
        }

        public TestsListViewModel() : base(c => c
               .DeleteAllowed()

                .Column(e => e.Sample, e=> e.SampleId)

                // todp : .Column(s => s.Sample.Product)

                .Column()
                    .Header("{Test}")
                    .Content(e => e.TestName)
                    .OrderBy(s => s.Order)
                    .Filter<EntityFilterNullable<TestClass>>()
                        .Header("{Test Class}")
                        .Link(e => e.TestClassId)

                .DescriptionColumn(t => "", t => t.Specification)
                    .Header("{Specifications}")
                    .OrderBy(s => s.Specification)

                .DescriptionColumn(t => "", t => t.Result?.Result)
                    .Header("{Result}")
                    .OrderBy(s => s.Result?.Result ?? "")

                .ConformityColumn(s => s.Result?.ConformityId)

                .StageColumn(default(SampleTestWorkflow), s => s.Stage)

                .Column().Hidden().Header("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Column().Hidden().Header("Group").Content(s => s.TestClassId)

        )
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

        }

        protected override bool CanExecuteDelete()
        {
            if (Selected == null) return false;
            if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
            if (!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return true;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}