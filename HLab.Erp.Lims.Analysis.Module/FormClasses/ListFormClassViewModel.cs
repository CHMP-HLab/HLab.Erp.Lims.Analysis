using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class ListFormClassViewModel : EntityListViewModel<FormClass>, IMvvmContextProvider
    {
        
        [Import] private readonly IErpServices _erp;

 
        public ListFormClassViewModel() 
        {
            AddAllowed = true;
            DeleteAllowed = true;

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                    .Column
                        .Header("{Name}")
                        .Width(200)
                        .Content(s => s.Name)
                        .Icon( s => s.IconPath)
                    .Column
                        .Header("{class}")
                        .Width(100)
                        .Content(s => s.Class??"")
                        .OrderBy(s => s.Class)
            );
                //.Hidden("IsValid",  s => s.Validation != 2)
                ;
            using (List.Suspender.Get())
            {
            }
        }

        public override string Title => "{Forms}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
