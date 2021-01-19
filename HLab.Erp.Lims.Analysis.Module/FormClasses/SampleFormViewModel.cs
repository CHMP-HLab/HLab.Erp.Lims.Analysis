﻿using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    using H = H<SampleFormViewModel>;

    public class SampleFormViewModelDesign : SampleFormViewModel, IViewModelDesign
    {
    }

    public class SampleFormViewModel : EntityViewModel<SampleForm>
    {
        public SampleFormViewModel()
        {
            H.Initialize(this);
            FormHelper = new FormHelper();
        }

        [Import] public IErpServices Erp { get; set; }

        public bool IsReadOnly => _isReadOnly.Get();

        private readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
            .Set(e => !e.EditMode)
            .On(e => e.EditMode)
            .Update()
        );
        public bool EditMode => _editMode.Get();

        private readonly IProperty<bool> _editMode = H.Property<bool>(c => c
            .Set(e => 
                e.Locker != null 
                && e.Locker.IsActive 
                && e.Model.Sample.Stage == SampleWorkflow.Reception.Name
                && e.Erp.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
            )
            .On(e => e.Locker.IsActive)
            .NotNull(e => e.Locker)
            .Update()
        );


        public string Conformity => _conformity.Get();

        private readonly IProperty<string> _conformity = H.Property<string>(c => c
            .Set(e =>
                {
                    switch((int)e.Model.State)
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
            )
            .On(e => e.Model.State)
            .Update()
        );

        public string ConformityIconPath => _conformityIconPath.Get();

        private readonly IProperty<string> _conformityIconPath = H.Property<string>(c => c
            .Set(e =>
                {
                    switch((int)e.Model.State)
                    {
                        case -1 : return "Icons/Validations/Error";
                        case 0 : return "Icons/Results/NotChecked";
                        case 1 : return "Icons/Results/Running";
                        case 2 : return "Icons/Results/GaugeKO";
                        case 3 : return "Icons/Results/GaugeOK";
                        case 4 : return "Icons/Results/Invalidated";
                        default: return "Icons/Validations/Error";
                    } 
                }   )         
            .On(e => e.Model.State)
            .Update()
        );

        public SampleViewModel Parent
        {
            get => _parent.Get();
            set => _parent.Set(value);
        }
        private readonly IProperty<SampleViewModel> _parent = H.Property<SampleViewModel>();

        public FormHelper FormHelper
        {
            get => _formHelper.Get();
            set => _formHelper.Set(value);
        }
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>();

        private readonly ITrigger _ = H.Trigger(c => c
            .On(e => e.Model.Sample.Stage)
//            .On(e => e.Model.Values)
            .On(e => e.EditMode)
            .NotNull(e => e.Model)
            .Do(async e => await e.LoadAsync())
        );

        //public ITestHelper TestHelper => _testHelper.Get();
        //private readonly IProperty<ITestHelper> _testHelper = H.Property<ITestHelper>(c => c
        //    .Set(e => e.FormHelper?.Form?.Test)
        //    .On(e => e.FormHelper.Form.Test)
        //    //.NotNull(e => e.FormHelper?.Form?.Test)
        //    .Update()
        //);

        //private ITrigger _1 = H.Trigger(c => c
        //    .On(e => e.TestHelper)
        //    .Do(e => e.TestHelper.PropertyChanged += e.TestHelper_PropertyChanged)
        //);

        //private void TestHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (Model==null) return;
        //    switch(e.PropertyName)
        //    {
        //        case "Result":
        //            if(TestHelper?.Result!=null)
        //                Model.Result = TestHelper.Result;
        //            break;
        //        case "Conformity":
        //            if(TestHelper?.Result!=null)
        //                Model.Conformity = TestHelper.Conformity;
        //            break;
        //        case "State":
        //                Model.StateId = (int)TestHelper.State;
        //            break;
        //        //case "MandatoryDone":
        //        //    Model.MandatoryDone = TestHelper.MandatoryDone;
        //        //    break;
        //    }
        //}
        public async Task LoadAsync()
        {
            await FormHelper.LoadCodeAsync(Model.FormClass.Code).ConfigureAwait(true);

            await FormHelper.LoadFormAsync(Model).ConfigureAwait(true);

            FormHelper.Mode = Model.Sample.Stage == SampleWorkflow.Reception.Name ? FormMode.Capture : FormMode.ReadOnly;

            FormHelper.LoadValues(Model.SpecValues);
            FormHelper.LoadValues(Model.Values);
        }

        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .Set(e => e.Model.Sample?.Reference + " - " + e.Model.FormClass.Name)
            .On(e => e.Model.Sample.Reference)
            .On(e => e.Model.FormClass.Name)
            .Update()
            );

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //    .Set(e => e.Model.SampleTest.TestName + "\n" + e.Model.SampleTest.Description)
        //    .On(e => e.Model.SampleTest.TestName)
        //    .On(e => e.Model.SampleTest.Description)
        //    .Update()
        //    );
    }
}
