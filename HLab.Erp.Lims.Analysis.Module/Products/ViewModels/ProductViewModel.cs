using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    using H = H<ProductViewModel>;

    internal class ProductViewModel: ListableEntityViewModel<Product>
    {

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.GetSubTitle )
            .On(e => e.Model.Variant)
            .On(e => e.Model.Form.Name)
            .Update()
        );
        private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.Form.IconPath)
        .On(e => e.Model.IconPath)
        .Update()
        );

        private string GetIconPath => Model?.Form?.IconPath??Model?.IconPath??base.IconPath;


        private readonly Func<Product, ProductProductComponentListViewModel> _getComponents;

        public ProductViewModel(Func<Product, ProductProductComponentListViewModel> getComponents)
        {
            _getComponents = getComponents;
            H.Initialize(this);
        }

        private readonly ITrigger _onEditMode = H.Trigger(c => c
            .On(e => e.Locker.IsActive)
            .Do(e =>
            {
                if(e.Components!=null)
                    e.Components.EditMode = e.Locker.IsActive;
            })
        );

        public ProductProductComponentListViewModel Components => _components.Get();
        private readonly IProperty<ProductProductComponentListViewModel> _components = H.Property<ProductProductComponentListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e =>
           {
               var components = e._getComponents?.Invoke(e.Model);
               //if (tests != null) tests.List.CollectionChanged += e.List_CollectionChanged;
               return components;
           })
            .On(e => e.Model)
            .Update()
        );

        //public ProductWorkflow Workflow => _workflow.Get();
        //private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
        //    .On(e => e.Model)
        //    .OnNotNull(e => e.Locker)
        //    .Set(vm => new ProductWorkflow(vm.Model,vm.Locker))
        //);
    }
    class ProductViewModelDesign : ProductViewModel, IViewModelDesign
    {
        public ProductViewModelDesign() : base(p => null)
        {
        }

        public new Product Model { get; } = Product.DesignModel;

    }
}
