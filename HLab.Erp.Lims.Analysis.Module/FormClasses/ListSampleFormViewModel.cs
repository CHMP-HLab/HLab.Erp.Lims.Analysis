using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class ListSampleFormViewModel : EntityListViewModel<SampleForm>, IMvvmContextProvider
    {
        [Import] private readonly IAclService _acl;


        public ListSampleFormViewModel(int sampleId)
        {
            List.AddFilter(()=>e => e.SampleId == sampleId);
            Columns
                .Icon("", s => s.FormClass.IconPath, s => s.FormClass.Name)
                .Column("{Name}", s => s.FormClass.Name);

            List.UpdateAsync();

            DeleteAllowed = true;
        }

        //protected override bool CanExecuteDelete()
        //{
        //    if(Selected==null) return false;
        //    if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
        //    if(!_acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
        //    return true;
        //}

        public override string Title => "Fiches";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
