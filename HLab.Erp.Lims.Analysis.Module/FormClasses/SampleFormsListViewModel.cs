
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;
using System;

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

        protected override bool CanExecuteAdd(Action<string> errorAction)
        {
            var stage = _sample.Stage.IsAny(errorAction,SampleWorkflow.Reception);
            var granted = Erp.Acl.IsGranted(errorAction,AnalysisRights.AnalysisReceptionCreate);
            return stage && granted;
        }

        protected override bool CanExecuteDelete(SampleForm target, Action<string> errorAction)
        {
            if (target == null) return false;
            var stage = target.Sample.Stage.IsAny(errorAction, SampleWorkflow.Reception);
            var granted = Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisReceptionCreate);
            return stage && granted;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
