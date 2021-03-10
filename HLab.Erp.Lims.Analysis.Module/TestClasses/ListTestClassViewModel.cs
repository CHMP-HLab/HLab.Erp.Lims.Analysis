using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{


    class ListTestClassViewModel : EntityListViewModel<TestClass>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public ListTestClassViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                .Column
                    .Header("{Name}").Width(200)
                    .Content(s => s.Name)
                    .Icon(s => s.IconPath)
                .Column
                    .Header("{Category}")
                    .Width(150)
                    .Content(s => s.Category?.Name??"")
                    .OrderBy(s => s.Category.Name)
            );
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {
                List.UpdateAsync();
            }
        }

        public override string Title => "{Test Classes}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
