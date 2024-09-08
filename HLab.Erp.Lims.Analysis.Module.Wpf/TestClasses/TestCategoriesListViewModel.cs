using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses;

internal class TestCategoriesListViewModel : Core.EntityLists.EntityListViewModel<TestCategory>, IMvvmContextProvider
{
    public TestCategoriesListViewModel(Injector i) : base(i, c => c
        //.AddAllowed()
        //.DeleteAllowed()
        .Column("Icon")
        .Width(80)
        .Icon(s => s.IconPath, 30 )
        .Column("Name")
        .Header("{Name}")
        .Width(200)
        .Content(s => s.Name)
    )
    {
    }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}