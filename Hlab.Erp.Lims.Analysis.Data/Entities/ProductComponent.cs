using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<ProductComponent>;

    public class ProductComponent : Entity
    {
        public ProductComponent() => H.Initialize(this);

        public int? ParentId
        {
            get => _parent.Id.Get();
            set => _parent.Id.Set(value);
        }

        [Ignore]
        public Product Parent
        {
            set => _parent.Set(value);
            get => _parent.Get();
        }
        private readonly IForeign<Product> _parent = H.Foreign<Product>();

        public int? ChildId
        {
            get => _child.Id.Get();
            set => _child.Id.Set(value);
        }

        [Ignore]
        public Product Child
        {
            set => _child.Set(value);
            get => _child.Get();
        }
        private readonly IForeign<Product> _child = H.Foreign<Product>();
    }
}