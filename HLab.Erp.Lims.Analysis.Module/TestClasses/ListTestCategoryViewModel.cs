using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class TestCategoriesListViewModel : EntityListViewModel<TestCategory>, IMvvmContextProvider
    {
        public TestCategoriesListViewModel() : base(c => ColumnConfiguratorExtension.Content(c
                //.AddAllowed()
                //.DeleteAllowed()
                .Column()
                .Width(80)
                .Icon(s => s.IconPath, 30 )
                .Column()
                .Header("{Name}")
                .Width(200), s => s.Name)
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
