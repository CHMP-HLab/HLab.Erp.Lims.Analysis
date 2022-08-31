using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests;

using H = H<SampleTestViewModel>;

public class SampleTestViewModelDesign : SampleTestViewModel, IViewModelDesign
{
    public SampleTestViewModelDesign() : base(null,null, null,null, null, null, null)
    {
    }
}

public class SampleTestViewModel : EntityViewModel<SampleTest>, IMvvmContextProvider
{
    public IDocumentService Docs { get; }
    readonly Func<Sample, DataLocker<Sample>> _getSampleLocker;
    public SampleTestViewModel(
        Injector i,
        IDocumentService docs,
        Func<SampleTest, TestResultsListViewModel> getResults,
        Func<int, SampleTestAuditTrailViewModel> getAudit,
        Func<FormHelper> getFormHelper,
        Func<SampleTest, IDataLocker<SampleTest>, SampleTestWorkflow> getSampleTestWorkflow,
        Func<Sample, DataLocker<Sample>> getSampleLocker
    ):base(i)
    {
        Docs = docs;
        _getResults = getResults;
        _getAudit = getAudit;
        _getFormHelper = getFormHelper;
        _getSampleTestWorkflow = getSampleTestWorkflow;
        _getSampleLocker = getSampleLocker;

        H.Initialize(this);
    }

    ITrigger _modelTrigger = H.Trigger(c => c.On(e => e.Model).Do(e => e.Locker.AddDependencyLocker(e._getSampleLocker(e.Model.Sample))));

    readonly Func<SampleTest,TestResultsListViewModel> _getResults;

    readonly Func<SampleTest, IDataLocker<SampleTest>, SampleTestWorkflow> _getSampleTestWorkflow;

    readonly Func<FormHelper> _getFormHelper;
    public FormHelper FormHelper => _formHelper.Get();

    readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
        .Set(e => e._getFormHelper?.Invoke()));


    public async Task LoadResultAsync(IFormTarget target=null)
    {
        await FormHelper.LoadAsync(target??Model).ConfigureAwait(true);

        var state = Workflow.CurrentStage;

        if (state == SampleTestWorkflow.Specifications) 
            FormHelper.Form.Mode = FormMode.Specification;
        //else if (state == SampleTestWorkflow.Running) 
        //    FormHelper.Mode = TestFormMode.Capture;
        else 
            FormHelper.Form.Mode = FormMode.ReadOnly;
    }



    // Audit Trail
    readonly Func<int, SampleTestAuditTrailViewModel> _getAudit;
    public SampleTestAuditTrailViewModel AuditTrail => _auditTrail.Get();

    readonly IProperty<SampleTestAuditTrailViewModel> _auditTrail = H.Property<SampleTestAuditTrailViewModel>(c => c
        .NotNull(e => e.Model)
        .Set(e => e._getAudit?.Invoke(e.Model.Id))
        .On(e => e.Model)
        .Update()
    );

    public bool AuditDetail
    {
        get => _auditDetail.Get();
        set => _auditDetail.Set(value);
    }

    readonly IProperty<bool> _auditDetail = H.Property<bool>();

    ITrigger _onAuditDetail = H.Trigger(c => c
        .On(e => e.AuditDetail)
        .On(e => e.AuditTrail)
        .NotNull(e => e.AuditTrail)
        .Do(e =>
        {
            if(e.AuditDetail)
                e.AuditTrail.List.RemoveFilter("Detail");
            else
                e.AuditTrail.List.AddFilter(e => e.Motivation != null || e.Log.Contains("Stage=") || e.Log.Contains("StageId="),0,"Detail");

            e.AuditTrail.List.Update();
        }));
    public SampleTestWorkflow Workflow => _workflow.Get();

    readonly IProperty<SampleTestWorkflow> _workflow = H.Property<SampleTestWorkflow>(c => c
        .NotNull(e => e.Locker)
        .Set(vm => vm._getSampleTestWorkflow?.Invoke(vm.Model,vm.Locker))
        .On(e => e.Model)
        .On(e => e.Locker)
        .Update()
    );


    public bool IsReadOnly => _isReadOnly.Get();

    readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
        .Set(e => !e.EditMode)
        .On(e => e.EditMode).Update()
    );

    public bool EditMode => _editMode.Get();

    readonly IProperty<bool> _editMode = H.Property<bool>(c => c
        .NotNull(e => e.Locker)
        .NotNull(e => e.Workflow)
        .NotNull(e => e.FormHelper?.Form)
        .Set(e => {
            e.FormHelper.Form.SetFormMode(e.FormHelper.Form.Mode);
            return e.Locker.IsActive
                   && e.Workflow.CurrentStage == SampleTestWorkflow.Specifications
                   && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisMonographSign);
        })
        .On(e => e.Locker.IsActive)
        .On(e => e.Workflow.CurrentStage)
        .On(e => e.FormHelper.Form)
        .Update()
    );
    public bool ScheduleEditMode => _scheduleEditMode.Get();

    readonly IProperty<bool> _scheduleEditMode = H.Property<bool>(c => c
        .NotNull(e => e.Locker)
        .NotNull(e => e.Workflow)
        .Set(e => 
            e.Locker.IsActive 
            && e.Workflow.CurrentStage == SampleTestWorkflow.Scheduling
            && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisSchedule)
        )
        .On(e => e.Locker.IsActive)
        .On(e => e.Workflow.CurrentStage).Update()
    );

    public bool ResultMode => _resultMode.Get();

    readonly IProperty<bool> _resultMode = H.Property<bool>(c => c
        .NotNull(e => e.Locker)
        .NotNull(e => e.Workflow)
        .Set(e => 
            e.Locker.IsActive 
            && e.Workflow.CurrentStage == SampleTestWorkflow.Running
            && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisResultEnter))
        .On(e => e.Locker.IsActive)
        .On(e => e.Workflow.CurrentStage)
        .Update()
         
    );
    public bool FormHelperIsActive => _formHelperIsActive.Get();

    readonly IProperty<bool> _formHelperIsActive = H.Property<bool>(c => c
        .Set(e => e.EditMode || e.ResultMode)
        .On(e => e.EditMode)
        .On(e => e.ResultMode).Update()
    );


    // RESULTS
    public TestResultsListViewModel Results => _results.Get();

    readonly IProperty<TestResultsListViewModel> _results = H.Property<TestResultsListViewModel>(c => c
        .NotNull(e => e.Model)
        .NotNull(e => e._getResults)
        .Set(e => e.SetResults())
        .On(e => e.Model)
        .Update()
    );

    TestResultsListViewModel SetResults()
    {
        var vm =  _getResults(Model);
        vm.SetSelectAction(async r =>
        {
            await LoadResultAsync(r as SampleTestResult).ConfigureAwait(false);
            if (SelectResultCommand is CommandPropertyHolder nc) nc.CheckCanExecute();
        });

        SampleTestResult selected = null;
        foreach(var result in vm.List)
        {
            if(selected==null) selected = result;
            if(result == Model.Result) selected = result;
        }
        vm.Selected = selected;
        return vm;
    }

    readonly ITrigger _trigger = H.Trigger(c => c
        .On(e => e.Model)
        .OnNotNull(e => e.Workflow)
        .OnNotNull(e => e.Results)
        .On(e => e.Locker.IsActive)
        .Do(async e => await e.LoadResultAsync(e.Results.Selected??e.Model.Result))
    );

    public ICommand ViewSpecificationsCommand { get; } = H.Command(c => c
        .Action(async e => await e.LoadResultAsync())
    );

    public ICommand AddResultCommand { get; } = H.Command(c => c
        .CanExecute(e => e._addResultCanExecute())
        .Action((e,t) => e.AddResult(e.Results.Selected))
        .On(e => e.Workflow.CurrentStage).CheckCanExecute()
    );
    public ICommand DeleteResultCommand { get; } = H.Command(c => c
        .CanExecute(e => e._deleteResultCanExecute())
        .Action((e,t) => e.DeleteResult(e.Results.Selected))
        .On(e => e.Workflow.CurrentStage)
        .On(e => e.Results.Selected.Stage)
        .On(e => e.Results.Selected)
        .On(e => e.Model.Result)
        .CheckCanExecute()
    );

    bool _addResultCanExecute()
    {
        if(!Injected.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
        if(Workflow.CurrentStage != SampleTestWorkflow.Running) return false;

        return true;
    }

    bool _deleteResultCanExecute()
    {
        if (Workflow == null) return false;
        if (Results?.Selected == null) return false;
        if(!Injected.Acl.IsGranted(AnalysisRights.AnalysisAddResult)) return false;
        if(Workflow.CurrentStage != SampleTestWorkflow.Running) return false;
        if (Results.Selected.Stage != null && Results.Selected.Stage != SampleTestResultWorkflow.Running) return false;
        if (Model.Result == null) return true;
        if(Model.Result.Id == Results.Selected.Id) return false;
        return true;
    }


    public ICommand SelectResultCommand { get; } = H.Command(c => c
        .CanExecute(e => 
            e.Results?.Selected?.Stage == SampleTestResultWorkflow.Validated
            && e.Model.Stage == SampleTestWorkflow.Running
            && e.Locker.IsActive
        )
        .Action(async (e, t) =>
        {
            if(
                e.Results?.Selected?.Stage == SampleTestResultWorkflow.Validated
                && e.Model.Stage == SampleTestWorkflow.Running
                && e.Locker.IsActive)
                await e.SelectResult(e.Results.Selected);
        })
        .On(e => e.Results.Selected.Stage)
        .On(e => e.Results.Selected)
        .On(e => e.Model.Stage)
        .On(e => e.Locker.IsActive)
        
        .CheckCanExecute()
    );

    public ICommand OpenSampleCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Docs.OpenDocumentAsync(e.Model.Sample);
        })
    );
    public ICommand OpenProductCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Docs.OpenDocumentAsync(e.Model.Sample.Product);
        })
    );
    public ICommand OpenCustomerCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Docs.OpenDocumentAsync(e.Model.Sample.Customer);
        })
    );

    async Task SelectResult(SampleTestResult result)
    {
        if(result.Stage == SampleTestResultWorkflow.Validated)
        {
            Model.Result = result;
            await Results.List.RefreshAsync();
        }
    }

    void AddResult(SampleTestResult previous)
    {
        int i = 0;

        foreach (var r in Results.List)
        {
            var n = r.Name??"";
            if (n.StartsWith("R")) n = n.Substring(1);

            if(int.TryParse(n, out var v))
            {
                i = Math.Max(i,v);
            }
        }            
            
        var test = Injected.Data.Add<SampleTestResult>(r =>
        {
            r.Name = string.Format("R{0}",i+1);
            r.SampleTest = Model;
            r.UserId = Model.UserId;
        });

        if (test != null)
            Results.List.Update();
    }

    void DeleteResult(SampleTestResult result)
    {
        if(_deleteResultCanExecute())
        {
            Injected.Data.Delete(result);

        }
    }


    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////
    /// </summary>

    public IFormTarget TestHelper => _testHelper.Get();

    readonly IProperty<IFormTarget> _testHelper = H.Property<IFormTarget>(c => c
        .NotNull(e => e.FormHelper?.Form?.Target)
        .Set(e => e.FormHelper.Form.Target)
        .On(e => e.FormHelper.Form.Target)
        .Update()
    );

        
    public override string Header => _header.Get();

    readonly IProperty<string> _header = H.Property<string>(c => c
        .Set(e => e.Model.Sample?.Reference)
        .On(e => e.Model.Sample.Reference)
        .Update()
    );

    public string SubTitle => _subTitle.Get();

    readonly IProperty<string> _subTitle = H.Property<string>(c => c
        .Set(e => e.Model.TestName + "\n" + e.Model.Description.TrimEnd('\r','\n',' '))
        .On(e => e.Model.TestName)
        .On(e => e.Model.Description)
        .Update()
    );


    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
//            ctx.AddCreator<SampleTestResultViewModel>(vm => vm.Parent = this);
    }
}