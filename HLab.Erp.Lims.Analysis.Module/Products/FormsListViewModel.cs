using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
   public class FormsListViewModel: EntityListViewModel<Form>, IMvvmContextProvider
    {
        public class FormDataModule : ErpParamBootloader<FormsListViewModel>
        {

        }
        public FormsListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()
                .Column().Header("{Name}").Content(e => e.Name).Filter<TextFilter>().Link(e => e.Name)
                .Column().Header("{Icon}").Icon((s) => s.IconPath).OrderBy(s => s.Name)            
        )
        {
        }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

    }
}