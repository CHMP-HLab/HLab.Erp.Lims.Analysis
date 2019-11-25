using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using Xceed.Wpf.AvalonDock.Controls;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    class ProductViewModel: EntityViewModel<ProductViewModel,Product>
    {
        [Import]
        public ObservableQuery<Form> Forms { get; }

        public string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c.OneWayBind(e => e.Model.Caption));

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
