using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class SampleFormsListViewModel : EntityListViewModel<SampleForm>, IMvvmContextProvider
    {
        private readonly Sample _sample;
        public SampleFormsListViewModel(Sample sample) : base(c => c
                .StaticFilter(e =>e.SampleId == sample.Id)
// TODO                .DeleteAllowed()
                .Column()
                    .Header("{Name}")
                    .Width(200)
                    .Content(s => s.FormClass.Name)
                    .Icon(s => s.FormClass.IconPath)
        )
        {
            _sample = sample;
        }

        protected override bool CanExecuteAdd()
        {
            if (_sample.Stage != SampleWorkflow.Reception.Name) return false;
            return Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionCreate);
        }

        protected override bool CanExecuteDelete()
        {
            if (Selected == null) return false;
            if (Selected.Sample.Stage != SampleWorkflow.Reception.Name) return false;
            return Erp.Acl.IsGranted(AnalysisRights.AnalysisReceptionCreate);
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
