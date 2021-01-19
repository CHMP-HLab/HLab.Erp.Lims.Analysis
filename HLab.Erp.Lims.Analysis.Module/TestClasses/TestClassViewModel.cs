using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    using H = H<TestClassViewModel>;
    public class TestCategoryViewModelDesign : TestCategoryViewModel, IViewModelDesign
    {
        public TestCategoryViewModelDesign()
        {


            Model = TestCategory.DesignModel;
        }
    }
    public class TestCategoryViewModel : EntityViewModel<TestCategory>
    {

    }

    public class TestClassViewModelDesign : TestClassViewModel, IViewModelDesign
    {
        public TestClassViewModelDesign()
        {
            Model = TestClass.DesignModel;
            FormHelper.Xaml = "<xml></xml>";
            FormHelper.Cs = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
        }
    }

    public class TestClassViewModel : EntityViewModel<TestClass>
    {
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.OneWayBind(e => e.Model.Name));

        public override string IconPath => Model.IconPath;

        public TestClassViewModel()
        {
            H.Initialize(this);
            FormHelper = new FormHelper();
        }

        public FormHelper FormHelper
        {
            get => _formHelper.Get();
            set => _formHelper.Set(value);
        }
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>();

        public string State
        {
            get => _state.Get();
            set => _state.Set(value);
        }
        private readonly IProperty<string> _state = H.Property<string>();

        public string TestName
        {
            get => _testName.Get();
            set => _testName.Set(value);
        }
        private readonly IProperty<string> _testName = H.Property<string>();

        public string Description
        {
            get => _description.Get();
            set => _description.Set(value);
        }
        private readonly IProperty<string> _description = H.Property<string>();

        public string Conformity
        {
            get => _conformity.Get();
            set => _conformity.Set(value);
        }
        private readonly IProperty<string> _conformity = H.Property<string>();

        public string Specifications
        {
            get => _specifications.Get();
            set => _specifications.Set(value);
        }
        private readonly IProperty<string> _specifications = H.Property<string>();

        public string Result
        {
            get => _result.Get();
            set => _result.Set(value);
        }
        private readonly IProperty<string> _result = H.Property<string>();

        public ICommand TryCommand { get; } = H.Command(c => c.Action(
            async e => await e.Compile()
        ));
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Mode = TestFormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
             e => e.FormHelper.Mode = TestFormMode.Capture
        ));

        public async Task Compile()
        {
            var specs = FormHelper.GetSpecPackedValues();
            var values = FormHelper.GetPackedValues();

            await FormHelper.LoadForm().ConfigureAwait(true);

            Model.Code = await FormHelper.SaveCode();

            FormHelper.LoadValues(specs);
            FormHelper.LoadValues(values);

            FormHelper.Form.Traitement(null,null);
        }

        //private IProperty<bool> _initLocker = H.Property<bool>(c => c.On(e => e.Locker).Do((e,f)=> {
        //    e.Locker.BeforeSavingAction = async t =>
        //    {
        //        var h = new FormHelper();
        //        h.Cs = e.Cs;
        //        h.Xaml = e.Xaml;
        //        t.Code = await h.SaveCode();
        //    };
        //}));

        private ITrigger _init = H.Trigger(c => c.On(e => e.Model).Do(async (e, f) =>
        {
            if (e.Model.Code != null)
            {
                await e.FormHelper.ExtractCode(e.Model.Code).ConfigureAwait(true);
            }
            else
            {
                e.FormHelper.Xaml = "<Grid></Grid>";
                e.FormHelper.Cs = "public class Test\n{\n}";
            }

            await e.Compile();
        }));

    }
}
