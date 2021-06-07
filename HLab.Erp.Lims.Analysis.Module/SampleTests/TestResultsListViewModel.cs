using System;
using System.Threading.Tasks;

using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class TestResultsListViewModel : EntityListViewModel<SampleTestResult>, IMvvmContextProvider
    {
        private readonly SampleTest _sampleTest;

        public TestResultsListViewModel(SampleTest sampleTest) : base(c => c
                .StaticFilter(e => e.SampleTestId == sampleTest.Id)

                .Column()
                    .Header("{Name}")
                    .Link(s => s.Name)
                    .Width(70)

                .Column()
                    .Header("{Start}")
                    .Link(s => s.Start)
                    .Width(80)//.OrderByOrder(0)
                
                .Column()
                    .Header("{End}")
                    .Link(s => s.End)
                    .Width(80)

                .Column()
                    .Header("{Result}")
                    .Link(s => s.Result)
                    .Width(80)

                .ConformityColumn(s => s.ConformityId)
                
                .StageColumn(default(SampleTestResultWorkflow), s => s.StageId)

                .Column()
                    .Hidden()
                    .Id("IsSelected")
                    .Content(s => s.Id == s.SampleTest.Result?.Id)

                .Column()
                    .Hidden()
                    .Id("IsValid")
                    .Content(s => s.Stage != SampleTestResultWorkflow.Invalidated)

        )
        {
            _sampleTest = sampleTest;
        }

        protected override async Task AddEntityAsync()
        {
            var target = Selected;

            int i = 0;

            foreach (var r in List)
            {
                var n = r.Name;
                if (n.StartsWith("R")) n = n.Substring(1);

                if (int.TryParse(n, out var v))
                {
                    i = Math.Max(i, v);
                }
            }


            var result = await Erp.Data.AddAsync<SampleTestResult>(r =>
           {
               r.Name = $"R{i + 1}";
               r.SampleTestId = _sampleTest.Id;
               if (target != null)
               {

               }
           });
            if (result != null)
            {
                List.Update();
                await Erp.Docs.OpenDocumentAsync(result);
            }

        }

        protected override bool CanExecuteDelete(SampleTestResult result, Action<string> errorAction)
        {
            if (Selected == null) return false;
            if (!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            if (_sampleTest.Stage != SampleTestWorkflow.Running) return false;
            if (Selected.Stage != null && Selected.Stage != SampleTestResultWorkflow.Running) return false;
            if (_sampleTest.Result == null) return true;
            if (_sampleTest.Result.Id == Selected.Id) return false;
            return true;
        }

        protected override bool CanExecuteAdd(Action<string> errorAction)
        {
            if (_sampleTest.Stage != SampleTestWorkflow.Running) return false;
            if (!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            return true;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
