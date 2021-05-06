using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class SampleFormsListViewModel : EntityListViewModel<SampleForm>, IMvvmContextProvider
    {
        public SampleFormsListViewModel(int sampleId) : base(c => ColumnConfiguratorExtension.Content(c
                .StaticFilter(e =>e.SampleId == sampleId)
// TODO                .DeleteAllowed()
                .Column()
                .Header("{Name}")
                .Width(200), s => s.FormClass.Name)
                           .Icon(s => s.FormClass.IconPath)
        )
        {
        }

        //protected override bool CanExecuteDelete()
        //{
        //    if(Selected==null) return false;
        //    if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
        //    if(!_acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
        //    return true;
        //}


        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
