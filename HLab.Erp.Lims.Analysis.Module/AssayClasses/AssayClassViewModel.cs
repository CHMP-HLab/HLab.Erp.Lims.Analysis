using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.AssayClasses
{
    public class AssayClassViewModelDesign : AssayClassViewModel, IViewModelDesign
    {
        public AssayClassViewModelDesign()
        {
            Model = AssayClass.DesignModel;
            Model.Xaml = "<xml></xml>";
            Model.CodeBehind = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
        }
    }

    public class AssayClassViewModel : EntityViewModel<AssayClassViewModel, AssayClass>
    {
        public string Title => Model.Name;
        public object Form
        {
            get => _form.Get();
            set => _form.Set(value);
        }
        private readonly IProperty<object> _form = H.Property<object>();

        public string Xaml
        {
            get => _xaml.Get();
            set => Model.Xaml = value;
        }
        private readonly IProperty<string> _xaml = H.Property<string>(c => c.OneWayBind(e => e.Model.Xaml));

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
        private readonly IProperty<string> _cs = H.Property<string>(c => c.OneWayBind(e => e.Model.CodeBehind));

        public string CsMessage
        {
            get => _csMessage.Get();
            set => _csMessage.Set(value);
        }
        private readonly IProperty<string> _csMessage = H.Property<string>();

        public ICommand TryCommand { get; } = H.Command(c => c.Action(
            async e => await e.Compile()
        ));

        public async Task Compile()
        {
            var h = new FormHelper();

            await h.ExtractCode(Model.Code).ConfigureAwait(true);
            Form = await h.LoadForm().ConfigureAwait(true);

            XamlMessage = h.XamlMessage;
            CsMessage = h.CsMessage;
        }

        private IProperty<bool> _init = H.Property<bool>(c => c.On(e => e.Model).Do(async (e, f) =>
            { 
                await e.Compile();
        }));

    }
}
