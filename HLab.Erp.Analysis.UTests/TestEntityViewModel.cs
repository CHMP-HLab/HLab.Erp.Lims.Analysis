using System;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Analysis.UTests
{
    using H = H<TestEntityViewModel>;   
    
    public class TestEntityViewModel : EntityViewModel<SampleTestResult>
    {
        public TestEntityViewModel(
            Func<FormHelper> getFormHelper,
            Func<SampleTestResult, IDataLocker<SampleTestResult>, SampleTestResultWorkflow> getWorkflow,
            Func<SampleTestResult, LinkedDocumentsListViewModel> getDocuments):base(null)
        {
            _getFormHelper = getFormHelper;
            _getWorkflow = getWorkflow;
            _getDocuments = getDocuments;

            H.Initialize(this);
        }

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


        public bool EditMode => _editMode.Get();

        readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Workflow)
            .NotNull(e => e.Locker)
            .Set(e =>
                e.Locker.IsActive
                && e.Workflow.CurrentStage == SampleTestResultWorkflow.Running
                && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );


        readonly Func<FormHelper> _getFormHelper;
        public FormHelper FormHelper => _formHelper.Get();

        readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
            .Set(e => e._getFormHelper?.Invoke()));


        readonly Func<SampleTestResult, LinkedDocumentsListViewModel> _getDocuments;
    }
}