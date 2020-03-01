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
        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
        .On(e => e.Model.Inn)
        .Set(e => e.Model?.Inn ));

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        .On(e => e.Model.Dose)
        .On(e => e.Model.Form.Name)
        .Set(e => e.getSubTitle ));
        private string getSubTitle => Model?.Dose + "\n" + Model?.Form?.Name;


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .On(e => e.Model.Form.IconPath)
        .On(e => e.Model.IconPath)
        .Set(e => e.getIconPath ));

        private string getIconPath => Model?.Form?.IconPath??Model.IconPath??base.IconPath;

        //public ProductWorkflow Workflow => _workflow.Get();
        //private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
        //    .On(e => e.Model)
        //    .OnNotNull(e => e.Locker)
        //    .Set(vm => new ProductWorkflow(vm.Model,vm.Locker))
        //);
    }
    class ProductViewModelDesign : ProductViewModel, IViewModelDesign
    {
        public new Product Model { get; } = Product.DesignModel;

    }
}
