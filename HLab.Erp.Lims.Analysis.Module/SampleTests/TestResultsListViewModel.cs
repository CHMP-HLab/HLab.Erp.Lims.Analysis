using System;
using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests;

using H = H<TestResultsListViewModel>;

public class TestResultsListViewModel : Core.EntityLists.EntityListViewModel<SampleTestResult>, IMvvmContextProvider
{
    readonly IAclService _acl;
    readonly IDataService _data;
    readonly IDocumentService _docs;

    public SampleTest SampleTest { get; }

    public TestResultsListViewModel(IDataService data, IAclService acl, IDocumentService docs, Injector i, SampleTest sampleTest) : base(i, c => c
        .StaticFilter(e => e.SampleTestId == sampleTest.Id)

        .Column("Selected")
        .Header("{Selected}")
        .Icon(s => (s.SampleTest.ResultId == s.Id) ? "Icons/Conformity/Selected" : "Icons/Conformity/NotSelected",20)
        .Width(70)

        .Column("Name")
        .Header("{Name}")
        .Link(s => s.Name)
        .Width(70)

        .Column("Start")
        .Header("{Start}")
        .Link(s => s.Start)
        .Width(100)//.OrderByOrder(0)
                
        .Column("End")
        .Header("{End}")
        .Link(s => s.End)
        .Width(100)

        .Column("Result")
        .Header("{Result}")
        .Link(s => s.Result)
        .Width(80)

        .ConformityColumn(s => s.ConformityId)
                
        .StageColumn(default(SampleTestResultWorkflow), s => s.StageId)

        .Column("IsSelected")
        .Hidden()
        .Content(s => s.Id == s.SampleTest.Result?.Id)

        .Column("IsValid")
        .Hidden()
        .Content(s => s.Stage != SampleTestResultWorkflow.Invalidated)

    )
    {
        _data = data;
        _acl = acl;
        _docs = docs;
        SampleTest = sampleTest;

        H.Initialize(this);
    }

    protected override async Task AddEntityAsync()
    {
        var target = Selected;

        int i = 0;

        foreach (var r in List)
        {
            // Todo : more robust parsing (should deal with any aother prefix)
            var n = r.Name;
            if (n.StartsWith("R",StringComparison.InvariantCulture))
            {
                n = n[1..];
            }

            if (int.TryParse(n, out var v))
            {
                if (v > i)
                {
                    i = v;
                }
            }
        }


        var result = await _data.AddAsync<SampleTestResult>(r =>
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
            await List.UpdateAsync();
            await _docs.OpenDocumentAsync(result);
        }
    }

    protected override bool CanExecuteDelete(SampleTestResult result, Action<string> errorAction)
    {
        if (Selected == null) return false;
        if (!_acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
        if (SampleTest.Stage != SampleTestWorkflow.Running) return false;
        if (Selected.Stage != null && Selected.Stage != SampleTestResultWorkflow.Running) return false;
        if (SampleTest.Result == null) return true;
        if (SampleTest.Result.Id == Selected.Id) return false;
        return true;
    }

    readonly ITrigger _ = H.Trigger(c => c
        .On(e => e.SampleTest.Stage).Do(e => (e.AddCommand as CommandPropertyHolder)?.CheckCanExecute())
    );

    protected override bool CanExecuteAdd(Action<string> errorAction)
    {
        if (SampleTest.Stage != SampleTestWorkflow.Running) return false;
        if (!_acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
        return true;
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}