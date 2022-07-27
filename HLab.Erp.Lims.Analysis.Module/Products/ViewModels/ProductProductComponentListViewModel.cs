using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Mvvm.Annotations;
using System;
using System.Windows.Input;
using HLab.Notify.PropertyChanged;
using System.Threading.Tasks;
using HLab.Erp.Base.Data;
/* Modification non fusionnée à partir du projet 'HLab.Erp.Lims.Analysis.Module (net6.0-windows)'
Avant :
using HLab.Erp.Lims.Analysis.Data.Entities;
Après :
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab;
using HLab.Erp;
using HLab.Erp.Lims;
using HLab.Erp.Lims.Analysis;
using HLab.Erp.Lims.Analysis.Module;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.Products.ViewModels;
*/
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Module.Pharmacopoeias;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    using H = H<ProductProductComponentsListViewModel>; 
    public class ProductProductComponentsListViewModel : Core.EntityLists.EntityListViewModel<ProductComponent>, IMvvmContextProvider
    {
        public Product Product { get; }
        public ProductProductComponentsListViewModel(Injector i, Product product) : base(i, c => c
                //.DeleteAllowed()
                .StaticFilter(e => e.ProductId == product.Id)

               .Column("Inn")
               .Header("{Inn}")
               .Width(100)
               .Content(e => e.Inn == null ? "" : e.Inn.Name)
               .OrderBy(e => e.Inn?.Name)

               .Column("Dose")
               .Header("{Dose}")
               .Width(100)
               .Content(e => e.Quantity)
               .OrderBy(e => e.Quantity)

               .Column("Unit")
               .Header("{Unit}")
               .Width(100)
               .Content(e => e.Unit?.Symbol)
               .OrderBy(e => e.Unit?.Symbol)
        )
        {

            Product = product;

            H.Initialize(this);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
        }

        public bool EditMode { get => _editMode.Get(); set => _editMode.Set(value); }
        readonly IProperty<bool> _editMode = H.Property<bool>(c => c.Default(false));

        protected override bool CanExecuteDelete(ProductComponent sampleTest, Action<string> errorAction)
        {
            if (!EditMode) return false;
            if (sampleTest == null) return false;
            //var stage =  sampleTest.Stage.IsAny( errorAction, SampleTestWorkflow.Specifications);
            //var granted = Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest);
            //return stage && granted;
            return true;
        }

        public override Type AddArgumentClass => typeof(Inn);
        public Type AddListClass => typeof(InnsListViewModel);

        readonly ITrigger _1 = H.Trigger(c => c
            //.On(e => e.Sample.Stage)
            //.On(e => e.Sample.Pharmacopoeia)
            //.On(e => e.Sample.PharmacopoeiaVersion)
            .On(e => e.EditMode)
            .Do(e => (e.AddCommand as CommandPropertyHolder)?.CheckCanExecute())
        );

        //private readonly ITrigger _triggerConformity = H.Trigger(c => c
        //    .On(e => e.List.Item().Result.ConformityId)
        //    .On(e => e.List.Item().Result.Stage)
        //    .On(e => e.List.Item().Result.Start)
        //    .On(e => e.List.Item().Result.End)
        //    .Do(e => e.List.Refresh())
        //);

        protected override bool CanExecuteAdd(Action<string> errorAction)
        {
            if (!EditMode) return false;
            if (!Injected.Erp.Acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest)) return false;
            //if (Sample.Pharmacopoeia == null)
            //{
            //    errorAction("{Missing} : {Pharmacopoeia}");
            //    return false;
            //}
            //if (string.IsNullOrWhiteSpace(Sample.PharmacopoeiaVersion))
            //{
            //    errorAction("{Missing} : {Pharmacopoeia version}");
            //    return false;
            //}
            //if (! Sample.Stage.IsAny(errorAction,SampleWorkflow.Monograph))
            //{
            //    errorAction("{requier stage} : {Monograph}");
            //    return false;
            //}
            return true;
        }

        protected override async Task AddEntityAsync(object arg)
        {
            if (arg is not Inn inn) return;

            var unit = await Injected.Erp.Data.FetchOneAsync<Unit>(e => e.Symbol=="mg");

            var component = await Injected.Erp.Data.AddAsync<ProductComponent>(pc =>
            {
                pc.Product = Product;
                pc.Inn = inn;
                pc.Unit = unit;
            });

            if (component != null) List.Update();
        }


        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}