using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleSampleTestListViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        [Import] private readonly IAclService _acl;


        public SampleSampleTestListViewModel(int sampleId)
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            List.AddFilter(()=>e => e.SampleId == sampleId);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                    
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
                .StageColumn(s => SampleTestWorkflow.StageFromName(s.Stage))

                .Column.Hidden.Id("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Column.Hidden.Id("Group").Content(s => s.TestClassId)
            );

            List.UpdateAsync();

            DeleteAllowed = true;
        }

        protected override bool CanExecuteDelete()
        {
            if(Selected==null) return false;
            if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
            if(!_acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return true;
        }

        public override string Title => "Samples";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}