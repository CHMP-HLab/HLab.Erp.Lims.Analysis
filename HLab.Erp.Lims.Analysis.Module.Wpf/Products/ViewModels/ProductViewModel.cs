using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels;

using H = H<ProductViewModel>;

internal class ProductViewModel: ListableEntityViewModel<Product>
{

    public string SubTitle => _subTitle.Get();

    readonly IProperty<string> _subTitle = H.Property<string>(c => c
        .Set(e => e.GetSubTitle )
        .On(e => e.Model.Variant)
        .On(e => e.Model.Form.Name)
        .Update()
    );

    string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


    public override string IconPath => _iconPath.Get();

    readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.Form.IconPath)
        .On(e => e.Model.IconPath)
        .Update()
    );

    string GetIconPath => Model?.Form?.IconPath??Model?.IconPath??base.IconPath;


    readonly Func<Product, ProductProductComponentsListViewModel> _getComponents;

    public ProductViewModel(Injector i, Func<Product, ProductProductComponentsListViewModel> getComponents):base(i)
    {
        _getComponents = getComponents;
        H.Initialize(this);
    }

    readonly ITrigger _onEditMode = H.Trigger(c => c
        .On(e => e.Locker.IsActive)
        .Do(e =>
        {
            if(e.Components!=null)
                e.Components.EditMode = e.Locker.IsActive;
        })
    );

    public ProductProductComponentsListViewModel Components => _components.Get();

    readonly IProperty<ProductProductComponentsListViewModel> _components = H.Property<ProductProductComponentsListViewModel>(c => c
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

internal class ProductViewModelDesign : ProductViewModel, IViewModelDesign
{
    public ProductViewModelDesign() : base(null,p => null)
    {
    }

    public new Product Model { get; } = Product.DesignModel;

}