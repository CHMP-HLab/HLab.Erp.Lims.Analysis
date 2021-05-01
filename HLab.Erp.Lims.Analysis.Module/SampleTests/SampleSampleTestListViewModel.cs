using Grace.DependencyInjection.Attributes;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleSampleTestListViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        private int _sampleId;
        public SampleSampleTestListViewModel(int sampleId) : base(c => c
                .DeleteAllowed()
            
                .DescriptionColumn(s=>s.TestName,s=>s.Description)
                    .Header("{Test}")//.Mvvm<IDescriptionViewClass>()
                .Width(300)
                .Icon(s => s.IconPath)
                .OrderBy(s => s.Order)

                .DescriptionColumn(s => "",s=>s.Specification)
                    .Header("{Specifications}")
                .Width(200)
                .OrderBy(s => s.Specification)

                .DescriptionColumn(s => "",s=>s.Result?.Result??"")
                
                    .Header("{Result}").Width(200).OrderBy(s => s.Result?.Result??"")

                .DescriptionColumn(s => "",s=>s.Result?.Conformity??"")
                
                    .Header("{Conformity}").Width(200).OrderBy(s => s.Result?.Conformity??"")


                .ConformityColumn(s => s.Result?.ConformityId)
                .StageColumn(default(SampleTestWorkflow),s => s.Stage)

                .Column().Hidden().Id("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Column().Hidden().Id("Group").Content(s => s.TestClassId)
        
        )
        {
            _sampleId = sampleId;
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
        }

        [Import]
        public void Inject()
        {
            List.AddFilter(()=>e => e.SampleId == _sampleId);
        }

        protected override bool CanExecuteDelete()
        {
            if(Selected==null) return false;
            if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return true;
        }


        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}