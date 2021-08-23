using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H1 = HD<ProductProductComponent>;
//    using H2 = HD<Inn>;

    public class ProductProductComponent : Entity
    {
        public ProductProductComponent() => H1.Initialize(this);

        public int? ProductId
        {
            get => _product.Id.Get();
            set => _product.Id.Set(value);
        }

        [Ignore]
        public Product Product
        {
            set => _product.Set(value);
            get => _product.Get();
        }
        private readonly IForeign<Product> _product = H1.Foreign<Product>();

    }

    //public class Inn : Entity
    //{
    //    public Inn() => H2.Initialize(this);

    //    public string Name
    //    {
    //        set => _name.Set(value);
    //        get => _name.Get();
    //    }
    //    private readonly IProperty<string> _name = H2.Property<string>();
    //}
}