using System;
using System.Threading.Tasks;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class TestResultListViewModel : EntityListViewModel<SampleTestResult>, IMvvmContextProvider
    {
        private SampleTest _sampleTest;

        public TestResultListViewModel Configure(SampleTest sampleTest)
        {
            _sampleTest = sampleTest;

            AddAllowed=true;
            DeleteAllowed=true;

            List.AddFilter(() => e => e.SampleTestId == sampleTest.Id);

             Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name).Width(70)
                .Column.Header("{Start}").Content(s => s.Start).Width(80).OrderByOrder(0)
                .Column.Header("{End}").Content(s => s.End).Width(80)
                .Column.Header("{Result}").Content(s => s.Result).Width(80)
                .ConformityColumn(s => s.ConformityId)
                .StageColumn(s => SampleTestResultWorkflow.StageFromName(s.Stage))

                .Column.Hidden.Id("IsSelected").Content(s => s.Id == s.SampleTest.Result?.Id)
                .Column.Hidden.Id("IsValid").Content(s => s.Stage != SampleTestResultWorkflow.Invalidated.Name)
                 );

            using (List.Suspender.Get())
            {
                DeleteAllowed = true;
                AddAllowed = true;
            }

            return this;
        }

        protected override void Configure()
        {
        }

        protected override async Task AddEntityAsync()
        {
            var target = Selected;

            int i = 0;

            foreach (var r in List)
            {
                var n = r.Name;
                if (n.StartsWith("R")) n = n.Substring(1);

                if(int.TryParse(n, out var v))
                {
                    i = Math.Max(i,v);
                }
            }


            var result  = await Erp.Data.AddAsync<SampleTestResult>(r =>
            {
                r.Name = $"R{i + 1}";
                r.SampleTestId = _sampleTest.Id;
                if(target!=null)
                {
                    
                }
            });
            if (result != null)
            {
                List.Update();
                await Erp.Docs.OpenDocumentAsync(result);
            }

        }

        protected override bool CanExecuteDelete()
        {
            if(Selected==null) return false;
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if(_sampleTest.Stage != SampleTestWorkflow.Running.Name) return false;
            if(Selected.Stage!=null && Selected.Stage != SampleTestResultWorkflow.Running.Name) return false;
            if(_sampleTest.Result==null) return true;
            if(_sampleTest.Result.Id == Selected.Id) return false;
            return true;
        }

        protected override bool CanExecuteAdd()
        {
            if (_sampleTest.Stage != SampleTestWorkflow.Running.Name) return false;
            if(!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            return true;
        }

        public override string Title => "{Results}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
