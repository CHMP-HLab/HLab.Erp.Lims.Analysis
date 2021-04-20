using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class ListSampleFormViewModel : EntityListViewModel<SampleForm>, IMvvmContextProvider
    {
        public ListSampleFormViewModel Configure(int sampleId)
        {
            List.AddFilter(()=>e => e.SampleId == sampleId);
            Columns.Configure(c => c
                        .Column
                            .Header("{Name}")
                            .Width(200)
                            .Content(s => s.FormClass.Name)
                            .Icon( s => s.FormClass.IconPath)
            );

            List.Update();

            DeleteAllowed = true;
            return this;
        }

        //protected override bool CanExecuteDelete()
        //{
        //    if(Selected==null) return false;
        //    if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
        //    if(!_acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
        //    return true;
        //}

        public override string Title => "Fiches";
        protected override void Configure()
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
