using HLab.Erp.Base.Data;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities
{
    using H = HD<ProductComponent>;

    public class ProductComponent : Entity, IListableModel
    {
        public ProductComponent() => H.Initialize(this);
        public string Caption => $"{Inn?.Caption} {Quantity,0} {Unit?.Symbol}";

        public int? ProductId
        {
            get => _product.Id.Get();
            set => _product.Id.Set(value);
        }

        public int? InnId
        {
            get => _inn.Id.Get();
            set => _inn.Id.Set(value);
        }
        public double Quantity
        {
            get => _quantity.Get();
            set => _quantity.Set(value);
        }

        readonly IProperty<double> _quantity = H.Property<double>(c => c.Default(0.0));
        public int? UnitId
        {
            get => _unit.Id.Get();
            set => _unit.Id.Set(value);
        }
        [Ignore]
        public Unit Unit
        {
            set => _unit.Set(value);
            get => _unit.Get();
        }

        readonly IForeign<Unit> _unit = H.Foreign<Unit>();

        [Ignore]
        public Product Product
        {
            set => _product.Set(value);
            get => _product.Get();
        }

        readonly IForeign<Product> _product = H.Foreign<Product>();

        [Ignore]
        public Inn Inn
        {
            set => _inn.Set(value);
            get => _inn.Get();
        }

        readonly IForeign<Inn> _inn = H.Foreign<Inn>();

        public static ProductComponent DesignModel => new()
        {
            Inn = Inn.DesignModel,
            Product = Product.DesignModel,
            Quantity = 100.0,
            Unit = Unit.DesignModel
        };
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