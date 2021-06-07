
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class TestCategoriesListViewModel : EntityListViewModel<TestCategory>, IMvvmContextProvider
    {
        public TestCategoriesListViewModel() : base(c => c
                //.AddAllowed()
                //.DeleteAllowed()
                .Column()
                    .Width(80)
                    .Icon(s => s.IconPath, 30 )
                .Column()
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

}
