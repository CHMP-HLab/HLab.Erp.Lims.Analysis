using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public class TestClassesListViewModel : EntityListViewModel<TestClass>, IMvvmContextProvider
    {
        public TestClassesListViewModel() : base(c => c
            //.AddAllowed()
            //.DeleteAllowed()
            .Column()
            .Header("{Name}").Width(200)
            .Content(s => s.Name)
            .Icon(s => s.IconPath)

            .Column()
            .Header("{Category}")
            .Width(150).Content(s => s.Category?.Name??"")
                    .OrderBy(s => s.Category?.Name)           
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
