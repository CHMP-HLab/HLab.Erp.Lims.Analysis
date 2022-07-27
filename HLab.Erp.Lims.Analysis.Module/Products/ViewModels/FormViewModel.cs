using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    using H = H<FormViewModel>;

    internal class FormViewModel: ListableEntityViewModel<Form>
    {
        public FormViewModel(Injector i) : base(i)
        {
        }
    }

    internal class FormViewModelDesign : FormViewModel, IViewModelDesign
    {
        public new Form Model { get; } = Form.DesignModel;

        public FormViewModelDesign() : base(null)
        {
        }
    }
}
