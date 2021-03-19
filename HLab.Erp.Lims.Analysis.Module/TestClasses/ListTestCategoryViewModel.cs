using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Icons.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class TestCategoriesListViewModel : EntityListViewModel<TestCategory>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public TestCategoriesListViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                    .Column
                        .Width(80)
                        .Icon(s => s.IconPath, 30 )
                    .Column
                        .Header("{Name}")
                        .Width(200)
                        .Content(s => s.Name)
            );
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {
            }
        }

        public override string Title => "{Test categories}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
