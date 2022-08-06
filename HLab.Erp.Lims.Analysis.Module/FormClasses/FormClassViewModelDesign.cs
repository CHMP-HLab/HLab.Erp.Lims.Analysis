using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

public class FormClassViewModelDesign : FormClassViewModel, IViewModelDesign
{
    public FormClassViewModelDesign() : base(null,null)
    {
//            Model = FormClass.DesignModel;
        FormHelper.Xaml = "<xml></xml>";
        FormHelper.Cs = "using HLab.Erp.Acl;\nusing HLab.Erp.Lims.Analysis.Data;\nusing HLab.Mvvm.Annotations;";
    }
}