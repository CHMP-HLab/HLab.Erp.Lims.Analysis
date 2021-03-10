using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Data;
using HLab.Erp.Forms.Annotations;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    using H = H<FormClassViewModel>;

    public class FormClassViewModelDesign : FormClassViewModel, IViewModelDesign
    {
        public FormClassViewModelDesign()
        {
//            Model = FormClass.DesignModel;
            FormHelper.Xaml = "<xml></xml>";
            FormHelper.Cs = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
        }
    }

    class DummyTarget : NotifierBase, IFormTarget
    {
        public FormState State
        {
            get =>_state.Get(); 
            set =>_state.Set(value); 
        }
        private readonly IProperty<FormState> _state = H<DummyTarget>.Property<FormState>();

        public string SpecValues
        {
            get => _specValues.Get(); 
            set => _specValues.Set(value);
        }
        private readonly IProperty<string> _specValues = H<DummyTarget>.Property<string>();

        public string Values
        {
            get => _values.Get(); 
            set => _values.Set(value);
        }
        private readonly IProperty<string> _values = H<DummyTarget>.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H<DummyTarget>.Property<bool>();

        public bool SpecificationsDone
        {
            get => _specificationsDone.Get(); 
            set => _specificationsDone.Set(value);
        }
        private readonly IProperty<bool> _specificationsDone = H<DummyTarget>.Property<bool>();    }

    public class FormClassViewModel : EntityViewModel<FormClass>
    {
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.Bind(e => e.Model.Name));

        public override string IconPath => Model.IconPath;

        public FormClassViewModel()
        {
            H.Initialize(this);
            FormHelper = new();
        }

        public FormHelper FormHelper
        {
            get => _formHelper.Get();
            private init => _formHelper.Set(value);
        }
        private readonly IProperty<FormHelper> _formHelper = H.Property<FormHelper>();

        public ICommand TryCommand { get; } = H.Command(c => c.Action(
            async e => await e.Compile()
        ));
        public ICommand SpecificationModeCommand { get; } = H.Command(c => c.Action(
            e => e.FormHelper.Mode = FormMode.Specification
        ));
        public ICommand CaptureModeCommand { get; } = H.Command(c => c.Action(
             e => e.FormHelper.Mode = FormMode.Capture
        ));

        public async Task Compile()
        {
            var specs = FormHelper.GetSpecPackedValues();
            var values = FormHelper.GetPackedValues();

            await FormHelper.LoadFormAsync(new DummyTarget()).ConfigureAwait(true);

            Model.Code = await FormHelper.SaveCode();

            FormHelper.LoadValues(specs);
            FormHelper.LoadValues(values);

            FormHelper.Form.Process(null,null);
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

        private ITrigger _init = H.Trigger(c => c
            .On(e => e.Model)
            .Do(async (e, f) =>
            {
                if (e.Model.Code != null)
                {
                    await e.FormHelper.LoadCodeAsync(e.Model.Code).ConfigureAwait(true);
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
