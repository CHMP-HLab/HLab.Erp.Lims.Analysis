using System;
using System.Threading.Tasks;

using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    using H = H<TestResultsListViewModel>;

    public class TestResultsListViewModel : EntityListViewModel<SampleTestResult>, IMvvmContextProvider
    {
        public SampleTest SampleTest { get; }

        public TestResultsListViewModel(SampleTest sampleTest) : base(c => c
                .StaticFilter(e => e.SampleTestId == sampleTest.Id)

                .Column()
                    .Header("{Selected}")
                    //.Content(s => s.SampleTest.ResultId == s.Id)
                    .Icon(s => (s.SampleTest.ResultId == s.Id) ? "Icons/Conformity/Selected" : "Icons/Conformity/NotSelected",20)
                    .Width(70)

                .Column()
                    .Header("{Name}")
                    .Link(s => s.Name)
                    .Width(70)

                .Column()
                    .Header("{Start}")
                    .Link(s => s.Start)
                    .Width(100)//.OrderByOrder(0)
                
                .Column()
                    .Header("{End}")
                    .Link(s => s.End)
                    .Width(100)

                .Column()
                    .Header("{Result}")
                    .Link(s => s.Result)
                    .Width(80)

                .ConformityColumn(s => s.ConformityId)//.UpdateOn(s => s.ConformityId)
                
                .StageColumn(default(SampleTestResultWorkflow), s => s.StageId)//.UpdateOn(s => s.StageId)

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
            SampleTest = sampleTest;

            H.Initialize(this);
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
               r.SampleTestId = SampleTest.Id;
               r.Start = DateTime.Now;
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
            if (SampleTest.Stage != SampleTestWorkflow.Running) return false;
            if (Selected.Stage != null && Selected.Stage != SampleTestResultWorkflow.Running) return false;
            if (SampleTest.Result == null) return true;
            if (SampleTest.Result.Id == Selected.Id) return false;
            return true;
        }

        private readonly ITrigger _ = H.Trigger(c => c
            .On(e => e.SampleTest.Stage).Do(e => (e.AddCommand as CommandPropertyHolder)?.CheckCanExecute())
        );

        //private readonly ITrigger _1 = H.Trigger(c => c
        //    .On(e => e.List.Item().Stage)
        //    .Do((e,a) => e.RefreshColumn("Stage"))
        //);

        protected override bool CanExecuteAdd(Action<string> errorAction)
        {
            if (SampleTest.Stage != SampleTestWorkflow.Running) return false;
            if (!Erp.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
            return true;
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
