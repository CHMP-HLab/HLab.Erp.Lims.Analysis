using System;
using System.Collections.Generic;
using System.Text;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using Xceed.Wpf.AvalonDock.Controls;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    class ProductViewModel: EntityViewModel<ProductViewModel,Product>
    {
        [Import]
        public ObservableQuery<Form> Forms { get; }

        public string Title => "Products";

        public ProductViewModel()
        {
            Forms.Update();
        }

    }
    class ProductViewModelDesign : ProductViewModel, IViewModelDesign
    {
        public new Product Model { get; } = Product.DesignModel;

    }
}
