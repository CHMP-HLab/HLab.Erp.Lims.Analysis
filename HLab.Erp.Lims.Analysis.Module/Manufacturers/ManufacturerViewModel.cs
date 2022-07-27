using HLab.Erp.Base.Wpf.Entities.Customers;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Data;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers
{
    public class ManufacturerViewModel : CorporationViewModel<Manufacturer>
    {
        public ManufacturerViewModel(Injector i) : base(i)
        {
        }
    }
}
