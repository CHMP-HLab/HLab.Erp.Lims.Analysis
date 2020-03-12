using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    class TestCategoriesListViewModel : EntityListViewModel<TestCategoriesListViewModel,TestCategory>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public TestCategoriesListViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s => new IconView { Path = s.IconPath, Width = 30 })
                .Column("{Name}", s => s.Name)
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {

                List.UpdateAsync();
            }
        }

        public override string Title => "{Test categories}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
