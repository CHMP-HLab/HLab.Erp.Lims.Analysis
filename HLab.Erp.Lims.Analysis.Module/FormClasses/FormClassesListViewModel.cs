using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{    
    public class FormClassesListViewModel : EntityListViewModel<FormClass>, IMvvmContextProvider
    {
        public FormClassesListViewModel() : base(c => c
// TODO                .AddAllowed()
// TODO                .DeleteAllowed()
            .Column()
            .Header("{Name}")
            .Width(200)
            .Content(s => s.Name)
            .Icon( s => s.IconPath)

            .Column()
            .Header("{class}")
            .Width(100).Content(s => s.Class??"")
                    .OrderBy(s => s.Class)
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
