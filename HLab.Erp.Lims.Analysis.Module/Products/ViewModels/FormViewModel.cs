using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    using H = H<FormViewModel>;

    class FormViewModel: ListableEntityViewModel<Form>
    {
    }

    class FormViewModelDesign : FormViewModel, IViewModelDesign
    {
        public new Form Model { get; } = Form.DesignModel;

    }
}
