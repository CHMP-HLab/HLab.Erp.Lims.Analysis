using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    class ProductCategoryViewModel: EntityViewModel<ProductCategory>
    {
        public ProductCategoryViewModel() => H<ProductCategoryViewModel>.Initialize(this);

        public override string Header => _header.Get();
        private readonly IProperty<string> _header = H<ProductCategoryViewModel>.Property<string>(c => c
            .Set(e => e.Model?.Name )
            .On(e => e.Model.Name)
            .Update()
        );

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //.On(e => e.Model.Dose)
        //.On(e => e.Model.Form.Name)
        //.Set(e => e.getSubTitle ));
        //private string getSubTitle => Model?.Dose + "\n" + Model?.Form?.Name;


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H<ProductCategoryViewModel>.Property<string>(c => c
            .Set(e => e.GetIconPath )
            .On(e => e.Model.IconPath).Update()
        );

        private string GetIconPath => Model.IconPath??base.IconPath;

    }
}