using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses;

public class TestCategoryViewModelDesign : TestCategoryViewModel, IViewModelDesign
{
    public TestCategoryViewModelDesign():base(null)
    {
        Model = TestCategory.DesignModel;
    }
}