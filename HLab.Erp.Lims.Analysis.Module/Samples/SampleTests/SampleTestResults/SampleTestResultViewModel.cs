using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HLab.Base.Extensions;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleMovements;
using HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTestResults;

using H = H<SampleTestResultViewModel>;

public class SampleTestResultViewModel : EntityViewModel<SampleTestResult>
{
    public class Design :  SampleTestResultViewModel, IViewModelDesign {}
    SampleTestResultViewModel():base(null) {}

    readonly Func<Sample, IDataLocker<Sample>> _getSampleLocker;
    readonly Func<SampleTest, IDataLocker<SampleTest>> _getSampleTestLocker;

    public SampleTestResultViewModel(
        Injector i,
        Func<FormHelper> getFormHelper, 
        Func<int, SampleTestResultAuditTrailViewModel> getAudit,
        Func<SampleTestResult, IDataLocker<SampleTestResult>, SampleTestResultWorkflow> getWorkflow,
        Func<SampleTestResult, LinkedDocumentsListViewModel> getDocuments,
        Func<SampleTestResult, SampleMovementsListViewModel> getMovements,
        Func<Sample, IDataLocker<Sample>> getSampleLocker,
        Func<SampleTest, IDataLocker<SampleTest>> getSampleTestLocker
    ):base(i)
    {
        _getFormHelper = getFormHelper;
        _getAudit = getAudit;
        _getWorkflow = getWorkflow;
        _getDocuments = getDocuments;
        _getMovements = getMovements;
        _getSampleLocker = getSampleLocker;
        _getSampleTestLocker = getSampleTestLocker;

        H.Initialize(this);
    }

    // Audit Trail
    readonly Func<int, SampleTestResultAuditTrailViewModel> _getAudit;
    public SampleTestResultAuditTrailViewModel AuditTrail => _auditTrail.Get();

    readonly IProperty<SampleTestResultAuditTrailViewModel> _auditTrail = H.Property<SampleTestResultAuditTrailViewModel>(c => c
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

    ITrigger _modelTrigger = H.Trigger(c => c
        .On(e => e.Model)
        .Do(e => e.Locker.AddDependencyLocker(
            e._getSampleLocker(e.Model.SampleTest.Sample),
            e._getSampleTestLocker(e.Model.SampleTest)
        )));

    readonly Func<SampleTestResult, IDataLocker<SampleTestResult>, SampleTestResultWorkflow> _getWorkflow;

    public SampleTestResultWorkflow Workflow => _workflow.Get();

    readonly IProperty<SampleTestResultWorkflow> _workflow = H.Property<SampleTestResultWorkflow>(c => c
        .NotNull(e => e.Locker)
        .NotNull(e => e.Model)
        .Set(e => e._getWorkflow?.Invoke(e.Model, e.Locker))
        .On(e => e.Model)
        .On(e => e.Locker)
        .Update()
    );

    public bool IsReadOnly => _isReadOnly.Get();

    readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
        .Set(e => !e.EditMode)
        .On(e => e.EditMode)
        .Update()
    );

    public bool EditMode => _editMode.Get();

    readonly IProperty<bool> _editMode = H.Property<bool>(c => c
        .NotNull(e => e.Workflow)
        .NotNull(e => e.Locker)
        .Set(e =>
            e.Locker.IsActive
            && e.Workflow.CurrentStage == SampleTestResultWorkflow.Running
            && e.Model.SampleTest.Stage == SampleTestWorkflow.Running
            && e.Model.SampleTest.Sample.Stage == SampleWorkflow.Production
            && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
        )
        .On(e => e.Locker.IsActive)
        .On(e => e.Workflow.CurrentStage)
        .Update()
    );


    public SampleTestViewModel Parent
    {
        get => _parent.Get();
        set => _parent.Set(value);
    }

    readonly IProperty<SampleTestViewModel> _parent = H.Property<SampleTestViewModel>();


    readonly Func<FormHelper> _getFormHelper;

    public FormHelper FormHelper => _formHelper.Get();

    readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
        .Set(e => e._getFormHelper?.Invoke()));

    readonly ITrigger _ = H.Trigger(c => c
        .On(e => e.Model.Stage)
        //            .On(e => e.Model.Values)
        .On(e => e.EditMode)
        .OnNotNull(e => e.Workflow)
        .Do(async e => await e.LoadResultAsync())
    );

    public async Task LoadResultAsync()
    {
        await FormHelper.LoadAsync(Model).ConfigureAwait(true);
        FormHelper.Form.Mode = Workflow.CurrentStage == SampleTestResultWorkflow.Running ? FormMode.Capture : FormMode.ReadOnly;
    }


    // Stock Movements
    readonly Func<SampleTestResult, SampleMovementsListViewModel> _getMovements;
    public SampleMovementsListViewModel Movements => _movements.Get();
    readonly IProperty<SampleMovementsListViewModel> _movements = H.Property<SampleMovementsListViewModel>(c => c
        .NotNull(e => e.Model)
        .Set(e => e._getMovements?.Invoke(e.Model))
        .On(e => e.Model)
        .Update()
    );


    // LINKED DOCUMENTS
    readonly Func<SampleTestResult, LinkedDocumentsListViewModel> _getDocuments;

    public LinkedDocumentsListViewModel LinkedDocuments => _linkedDocuments.Get();

    readonly IProperty<LinkedDocumentsListViewModel> _linkedDocuments = H.Property<LinkedDocumentsListViewModel>(c => c
        .NotNull(e => e.Model)
        .Set(e => e._getDocuments?.Invoke(e.Model).FluentAction(vm => vm.SetOpenAction(d => e.LinkedDocuments.Selected.OpenDocument())))
        .On(e => e.Model)
        .Update()
    );

    public ICommand PrintCommand { get; } = H.Command(c => c
        //.CanExecute(e => e._addDocumentCanExecute())
        .Action((e, t) => e.Print())
        .On(e => e.Workflow.CurrentStage).CheckCanExecute()
    );

    void Print()
    {
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() != true) return;

        if (FormHelper.Form is not Control visual) return;

        var f = visual.Foreground;
        var b = visual.Background;

        visual.Foreground = Brushes.Black;
        visual.Background = Brushes.White;

        var dic = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/light.blue.xaml") };

        visual.Resources.MergedDictionaries.Add(dic);

        printDialog.PrintVisual( visual, "My First Print Job");

        visual.Foreground = f;
        visual.Background = b;

        visual.Resources.MergedDictionaries.Remove(dic);
    }




    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// 
    public override string Header => _header.Get();

    readonly IProperty<string> _header = H.Property<string>(c => c
        .Set(e => e.Model.SampleTest?.Sample?.Reference + " - " + e.Model.Name)
        .On(e => e.Model.SampleTest.Sample.Reference)
        .On(e => e.Model.Name)
        .Update()
    );

    public string SubTitle => _subTitle.Get();

    readonly IProperty<string> _subTitle = H.Property<string>(c => c
        .Set(e => e.Model.SampleTest?.TestName + "\n" + e.Model.SampleTest?.Description.TrimEnd('\r', '\n', ' '))
        .On(e => e.Model.SampleTest.TestName)
        .On(e => e.Model.SampleTest.Description)
        .Update()
    );

    public ICommand OpenSampleCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Injected.Docs.OpenDocumentAsync(e.Model.SampleTest.Sample);
        })
    );
    public ICommand OpenProductCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Injected.Docs.OpenDocumentAsync(e.Model.SampleTest.Sample.Product);
        })
    );
    public ICommand OpenCustomerCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Injected.Docs.OpenDocumentAsync(e.Model.SampleTest.Sample.Customer);
        })
    );
    public ICommand OpenTestCommand { get; } = H.Command(c => c
        .Action(async (e, t) =>
        {
            await e.Injected.Docs.OpenDocumentAsync(e.Model.SampleTest);
        })
    );


}