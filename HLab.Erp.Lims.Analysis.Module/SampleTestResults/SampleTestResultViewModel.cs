using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTestResults
{
    public class SampleTestResultViewModelDesign : SampleTestResultViewModel, IViewModelDesign
    {
    }

    public class SampleTestResultViewModel : EntityViewModel<SampleTestResultViewModel,SampleTestResult>
    {
        public SampleTestResultViewModel()
        { }

        [Import] private Func<SampleTestResult, DataLocker<SampleTestResult>, SampleTestResultWorkflow> _getWorkflow;
        [Import] public IErpServices Erp { get; }

        public SampleTestResultWorkflow Workflow => _workflow.Get();
        private readonly IProperty<SampleTestResultWorkflow> _workflow = H.Property<SampleTestResultWorkflow>(c => c
            .On(e => e.Model)
            .On(e => e.Locker)
            .NotNull(e => e.Locker)
            .Set(e => e._getWorkflow(e.Model,e.Locker))
        );
        public bool IsReadOnly => _isReadOnly.Get();
        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .On(e => e.EditMode)
            .Set(e => !e.EditMode)
        );
        public bool EditMode => _editMode.Get();
        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .On(e => e.Locker.IsActive)
            .On(e => e.Workflow.CurrentState)
            .NotNull(e => e.Locker)
            .NotNull(e => e.Workflow)
            .Set(e => 
                e.Locker.IsActive 
                && e.Workflow.CurrentState == SampleTestResultWorkflow.Running
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
        );


        public string Conformity => _conformity.Get();
        private IProperty<string> _conformity = H.Property<string>(c => c
            .On(e => e.Model.StateId)
        .Set(e =>
            {
                switch(e.Model.StateId)
                {
                    case -1 : return "{Undefined}";
                    case 0 : return "{Not Started}";
                    case 1 : return "{Running}";
                    case 2 : return "{Not Conform}";
                    case 3 : return "{Conform}";
                    case 4 : return "{Not Valid}";
                    default: return "{error}";
                } 
            }            
        ));
        public string ConformityIconPath => _conformityIconPath.Get();
        private IProperty<string> _conformityIconPath = H.Property<string>(c => c
            .On(e => e.Model.StateId)
        .Set(e =>
            {
                switch(e.Model.StateId)
                {
                    case -1 : return "Icons/Validations/Error";
                    case 0 : return "Icons/Results/NotChecked";
                    case 1 : return "Icons/Results/Running";
                    case 2 : return "Icons/Results/GaugeKO";
                    case 3 : return "Icons/Results/GaugeOK";
                    case 4 : return "Icons/Results/Invalidated";
                    default: return "Icons/Validations/Error";
                } 
            }            
        ));

        public SampleTestViewModel Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }
        private readonly IProperty<SampleTestViewModel> _parent = H.Property<SampleTestViewModel>();
        public FormHelper FormHelper => _formHelper.Get();
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>(c => c
            .Default(new FormHelper()));

        private readonly IProperty<bool> _ = H.Property<bool>(c => c
            .On(e => e.Model.Stage)
            .OnNotNull(e => e.Workflow)
            .Do(async e => await e.LoadResultAsync())
        );

        public ITestHelper TestHelper => _testHelper.Get();
        private readonly IProperty<ITestHelper> _testHelper = H.Property<ITestHelper>(c => c
            .On(e => e.FormHelper.Form.Test)
            .NotNull(e => e.FormHelper?.Form)
            .Do((e,f) => {
                f.Set(e.FormHelper.Form.Test);
                e.TestHelper.PropertyChanged += e.TestHelper_PropertyChanged;
                })
        );

        private void TestHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Model==null) return;
            switch(e.PropertyName)
            {
                case "Result":
                    if(TestHelper?.Result!=null)
                        Model.Result = TestHelper.Result;
                    break;
                case "State":
                        Model.StateId = (int)TestHelper.State;
                    break;
                case "MandatoryDone":
                    Model.MandatoryDone = TestHelper.MandatoryDone;
                    break;
            }
        }

        public async Task LoadResultAsync()
        {
            await FormHelper.LoadAsync(Model).ConfigureAwait(true);

            var state = Workflow.CurrentState;

            if (state == SampleTestResultWorkflow.Running) 
                FormHelper.Mode = TestFormMode.Capture;
            else 
                FormHelper.Mode = TestFormMode.ReadOnly;
        }

        public override string Title => _title.Get();
        private IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.SampleTest.Sample.Reference)
            .On(e => e.Model.Name)
            .Set(e => e.Model.SampleTest.Sample?.Reference + " - " + e.Model.Name)
            );
        public string SubTitle => _subTitle.Get();
        private IProperty<string> _subTitle = H.Property<string>(c => c
            .On(e => e.Model.SampleTest.TestName)
            .On(e => e.Model.SampleTest.Description)
            .Set(e => e.Model.SampleTest.TestName + "\n" + e.Model.SampleTest.Description)
            );
    }
}
