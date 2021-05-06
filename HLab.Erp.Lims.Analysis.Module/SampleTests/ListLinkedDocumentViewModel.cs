using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grace.DependencyInjection.Attributes;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class LinkedDocumentsListViewModel : EntityListViewModel<LinkedDocument>, IMvvmContextProvider
    {
        private Action<LinkedDocument> _createAction;
        public LinkedDocumentsListViewModel(
            Expression<Func<LinkedDocument,bool>> filter,
            Action<LinkedDocument> createAction = null
            ) : base(c => c
            .StaticFilter(filter)
            //.DeleteAllowed()
            .Column()
            .Header("{Name}").Width(200).Content(s => s.Name)
        )
        {
            _createAction = createAction;
        }


        protected override bool CanExecuteDelete() => Selected != null;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
