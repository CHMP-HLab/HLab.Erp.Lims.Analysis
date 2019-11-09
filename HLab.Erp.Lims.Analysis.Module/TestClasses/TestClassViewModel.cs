using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public class TestClassViewModelDesign : TestClassViewModel, IViewModelDesign
    {
        public TestClassViewModelDesign()
        {
            Model = TestClass.DesignModel;
            Xaml = "<xml></xml>";
            Cs = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
        }
    }

    public class TestClassViewModel : EntityViewModel<TestClassViewModel, TestClass>
    {
        public string Title => Model.Name;
        public ITestForm Form
        {
            get => _form.Get();
            set => _form.Set(value);
        }
        private readonly IProperty<ITestForm> _form = H.Property<ITestForm>();

        public string Xaml
        {
            get => _xaml.Get();
            set => _xaml.Set(value);
        }
        private readonly IProperty<string> _xaml = H.Property<string>();

        public string XamlMessage
        {
            get => _xamlMessage.Get();
            set => _xamlMessage.Set(value);
        }
        private readonly IProperty<string> _xamlMessage = H.Property<string>();

        public string Cs
        {
            get => _cs.Get();
            set => _cs.Set(value);
        }
        private readonly IProperty<string> _cs = H.Property<string>();

        public string CsMessage
        {
            get => _csMessage.Get();
            set => _csMessage.Set(value);
        }
        private readonly IProperty<string> _csMessage = H.Property<string>();


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

        public async Task Compile()
        {

            var h = new FormHelper
            {
                Xaml = Xaml,
                Cs = Cs
            };

            Form = await h.LoadForm().ConfigureAwait(true);

            Model.Code = await h.SaveCode();

            XamlMessage = h.XamlMessage;
            CsMessage = h.CsMessage;
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

        private IProperty<bool> _init = H.Property<bool>(c => c.On(e => e.Model).Do(async (e, f) =>
        {

            if (e.Model.Code != null)
            {
                var h = new FormHelper();
                await h.ExtractCode(e.Model.Code).ConfigureAwait(true);
                e.Xaml = h.Xaml;
                e.Cs = h.Cs;
            }
            else
            {
                e.Xaml = "<Grid></Grid>";
                e.Cs = "class Test\n{\n}";
            }

            await e.Compile();
        }));

    }
}
