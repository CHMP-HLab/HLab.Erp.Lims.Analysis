using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    using H = H<ProductViewModel>;

    internal class ProductViewModel: EntityViewModel<Product>
    {
        public ProductViewModel() => H.Initialize(this);

        public override string Header => _header.Get();
        private readonly IProperty<string> _header = H.Property<string>(c => c
        .Set(e => string.IsNullOrWhiteSpace(e.Model?.Inn) ? "{New product}" : e.Model.Inn)
        .On(e => e.Model.Inn)
        .Update()
        );

        public string SubTitle => _subTitle.Get();
        private readonly IProperty<string> _subTitle = H.Property<string>(c => c
            .Set(e => e.GetSubTitle )
            .On(e => e.Model.Dose)
            .On(e => e.Model.Form.Name)
            .Update()
        );
        private string GetSubTitle => $"{Model?.Dose}\n{Model?.Form?.Name}";


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.Form.IconPath)
        .On(e => e.Model.IconPath)
        .Update()
        );

        private string GetIconPath => Model?.Form?.IconPath??Model?.IconPath??base.IconPath;

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
