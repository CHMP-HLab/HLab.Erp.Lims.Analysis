using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Manufacturers
{
    using H = NotifyHelper<ListManufacturerModule>;
    class ListManufacturerModule : IPostBootloader
    {
        private readonly IErpServices _erp;

        [Import] public ListManufacturerModule(IErpServices erp)
        {
            _erp = erp;
           H.Initialize(this);
        }
        public ICommand OpenListManufacturerCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListManufacturerViewModel))
        ));

        public void Load()
        {
            _erp.Menu.RegisterMenu("data", "manufacturers", "{Manufacturers}",
                OpenListManufacturerCommand,
                "icons/manufacturer");

        }    }
}
