using System;
using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;

using H = H<TestResultsListViewModel>;

public class TestResultsListViewModel : Core.EntityLists.EntityListViewModel<SampleTestResult>, IMvvmContextProvider
{
    readonly IAclService _acl;

    public SampleTest SampleTest { get; }

    public TestResultsListViewModel(IAclService acl, Injector i, SampleTest sampleTest) : base(i, c => c
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
        .Date(s => s.Start)
        .Link(s => s.Start)
        .Width(100)//.OrderByOrder(0)
                
        .Column("End")
        .Header("{End}")
        .Date(s => s.End)
        .Link(s => s.End)
        .Width(100)

        .Column("Result")
        .Header("{Result}")
        .Link(s => s.Result)
        .Width(80)

        .ConformityColumn(s => s.ConformityId)
                
        .StageColumn(default(SampleTestResultWorkflow), s => s.StageId)

        .AddProperty("IsSelected", s=> s is { Id: { }, SampleTest.Result.Id: { } }, s => s.Id == s.SampleTest.Result.Id)

        .AddProperty("IsValid", s=>s?.Stage != null, s => s.Stage != SampleTestResultWorkflow.Invalidated)

    )
    {
        _acl = acl;
        SampleTest = sampleTest;

        H.Initialize(this);

        ShowFilters = false;
    }

    protected override Task ConfigureNewEntityAsync(SampleTestResult result)
    {
        //var target = Selected;
        var i = 0;

        // find max 'Rxx' name value 
        foreach (var r in List)
        {
            // Todo : more robust parsing (should deal with any another prefix)
            var n = r.Name;
            if (n.StartsWith("R",StringComparison.InvariantCulture))
            {
                n = n[1..];
            }

            if (!int.TryParse(n, out var v)) continue;

            if (v > i) {i = v;}
        }

        result.Name = $"R{i + 1}";
        result.SampleTestId = SampleTest.Id;
        result.Start = DateTime.Now;

        return Task.CompletedTask;
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
        => SampleTest.Stage == SampleTestWorkflow.Running 
           && _acl.IsGranted(AnalysisRights.AnalysisAddResult);

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}