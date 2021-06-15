using System;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Analysis.UTests
{
    using H = H<TestEntityViewModel>;   
    
    public class TestEntityViewModel : EntityViewModel<SampleTestResult>
    {
        public TestEntityViewModel(
            Func<FormHelper> getFormHelper, 
            Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> getWorkflow,
            Func<SampleTestResult, LinkedDocumentsListViewModel> getDocuments)
        {
            _getFormHelper = getFormHelper;
            _getWorkflow = getWorkflow;
            _getDocuments = getDocuments;

            H.Initialize(this);
        }
        private readonly Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> _getWorkflow;
        public SampleTestResultWorkflow Workflow => _workflow.Get();

        private readonly IProperty<SampleTestResultWorkflow> _workflow = H.Property<SampleTestResultWorkflow>(c => c
            .NotNull(e => e.Locker)
            .NotNull(e => e.Model)
            .Set(e => e._getWorkflow?.Invoke(e.Model, e.Locker))
            .On(e => e.Model)
            .On(e => e.Locker)
            .Update()
        );


        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .NotNull(e => e.Workflow)
            .NotNull(e => e.Locker)
            .Set(e =>
                e.Locker.IsActive
                && e.Workflow.CurrentStage == SampleTestResultWorkflow.Running
                && e.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentStage)
            .Update()
        );



        private readonly Func<FormHelper> _getFormHelper;
        public FormHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
            .Set(e => e._getFormHelper?.Invoke()));




        private readonly Func<SampleTestResult, LinkedDocumentsListViewModel> _getDocuments;
    }
}