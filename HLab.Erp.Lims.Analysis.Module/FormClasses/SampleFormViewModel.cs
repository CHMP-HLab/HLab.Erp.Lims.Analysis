using System;
using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

using H = H<SampleFormViewModel>;

public class SampleFormViewModelDesign : SampleFormViewModel, IViewModelDesign
{
    public SampleFormViewModelDesign() : base(null,null)
    {
    }
}

public class SampleFormViewModel : ListableEntityViewModel<SampleForm>
{
    public SampleFormViewModel(Injector i, Func<FormHelper> getFormHelper) : base(i)
    {
        H.Initialize(this);
        FormHelper = getFormHelper();
    }


    public bool IsReadOnly => _isReadOnly.Get();

    readonly IProperty<bool> _isReadOnly = H.Property<bool>(c => c
        .Set(e => !e.EditMode)
        .On(e => e.EditMode)
        .Update()
    );
    public bool EditMode => _editMode.Get();

    readonly IProperty<bool> _editMode = H.Property<bool>(c => c
        .NotNull(e => e.Locker)
        .NotNull(e => e.Model.Sample)
        .Set(e => 
            e.Locker.IsActive 
            && e.Model.Sample.Stage == SampleWorkflow.Reception
            && e.Injected.Acl.IsGranted(AnalysisRights.AnalysisResultEnter)
        )
        .On(e => e.Locker.IsActive)
        .Update()
    );


    public string Conformity => _conformity.Get();

    readonly IProperty<string> _conformity = H.Property<string>(c => c
        .Set(e => e.Model.ConformityId.ToString())
        .On(e => e.Model.ConformityId)
        .Update()
    );

    public SampleViewModel Parent
    {
        get => _parent.Get();
        set => _parent.Set(value);
    }

    readonly IProperty<SampleViewModel> _parent = H.Property<SampleViewModel>();

    public FormHelper FormHelper
    {
        get => _formHelper.Get();
        set => _formHelper.Set(value);
    }

    readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>();

    readonly ITrigger _ = H.Trigger(c => c
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

    public async Task LoadAsync()
    {
        //await FormHelper.ExtractCodeAsync(Model.FormClass.Code).ConfigureAwait(true);

        //await FormHelper.LoadFormAsync(Model).ConfigureAwait(true);

        //FormHelper.Form.LoadValues(Model.SpecificationValues);
        //FormHelper.Form.LoadValues(Model.ResultValues);

        //FormHelper.Form.Mode = Model.Sample.Stage == SampleWorkflow.Reception ? FormMode.Capture : FormMode.ReadOnly;
        await FormHelper.LoadAsync(Model).ConfigureAwait(true);

        FormHelper.Form.Mode = Model.Sample.Stage == SampleWorkflow.Reception ? FormMode.Capture : FormMode.ReadOnly;

    }


    //public string SubTitle => _subTitle.Get();
    //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
    //    .Set(e => e.Model.SampleTest.TestName + "\n" + e.Model.SampleTest.Description)
    //    .On(e => e.Model.SampleTest.TestName)
    //    .On(e => e.Model.SampleTest.Description)
    //    .Update()
    //    );
}