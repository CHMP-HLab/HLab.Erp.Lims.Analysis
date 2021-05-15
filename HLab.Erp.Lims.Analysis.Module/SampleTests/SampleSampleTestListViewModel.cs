using Grace.DependencyInjection.Attributes;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleSampleTestListViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        public SampleSampleTestListViewModel(Sample sample) : base(c => c
                //.DeleteAllowed()
                .StaticFilter(e => e.SampleId == sample.Id)

                .DescriptionColumn(s => s.TestName, s => s.Description)
                    .Header("{Test}")//.Mvvm<IDescriptionViewClass>()
                .Width(300)
                .Icon(s => s.IconPath)
                .OrderBy(s => s.Order)

                .DescriptionColumn(s => "", s => s.Specification)
                    .Header("{Specifications}")
                .Width(200)
                .OrderBy(s => s.Specification)

                .DescriptionColumn(s => "", s => s.Result?.Result ?? "")

                    .Header("{Result}").Width(200).OrderBy(s => s.Result?.Result ?? "")

                .DescriptionColumn(s => "", s => s.Result?.Conformity ?? "")

                    .Header("{Conformity}").Width(200).OrderBy(s => s.Result?.Conformity ?? "")


                .ConformityColumnPostLinked(s => s.Result != null ? s.Result.ConformityId : ConformityState.NotChecked)

                .StageColumn(default(SampleTestWorkflow), s => s.Stage)

                .Column().Hidden().Id("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Column().Hidden().Id("Group").Content(s => s.TestClassId)

        )
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
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