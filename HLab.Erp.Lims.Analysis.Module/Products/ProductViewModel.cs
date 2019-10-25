using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using Xceed.Wpf.AvalonDock.Controls;

namespace HLab.Erp.Lims.Analysis.Module.Products
{

    public class ProductWorkflow : Workflow<ProductWorkflow,Product>
    {
        public ProductWorkflow(Product product):base(product)
        {
            H.Initialize(this);

            CurrentState = Created;
            Update();
        }

        public static State Created = State.Create(c => c
            .Caption("^Reception entry").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Created)
        );

    }

    class ProductViewModel: EntityViewModel<ProductViewModel,Product>
    {
        [Import]
        public ObservableQuery<Form> Forms { get; }


        public ProductViewModel()
        {
            Forms.Update();
        }
        public ProductWorkflow Workflow => _workflow.Get();
        private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
            .On(e => e.Model)
            .Set(vm => new ProductWorkflow(vm.Model))
        );

    }
    class ProductViewModelDesign : ProductViewModel, IViewModelDesign
    {
        public new Product Model { get; } = Product.DesignModel;

    }
}
