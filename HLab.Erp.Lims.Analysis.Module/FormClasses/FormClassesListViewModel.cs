using System.Windows.Input;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{    
    public class FormClassesListViewModel : EntityListViewModel<FormClass>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        protected override bool CanExecuteAdd() => Erp.Acl.IsGranted(AnalysisRights.AnalysisProductCreate);
        protected override bool CanExecuteDelete() => Erp.Acl.IsGranted(AnalysisRights.AnalysisProductCreate);

        public FormClassesListViewModel() : base(c => c
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
