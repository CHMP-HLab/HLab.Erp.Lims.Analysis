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
    class ListManufacturerModule : N<ListManufacturerModule>, IBootloader //postboot
    {
        [Import] private readonly IErpServices _erp;

       public ICommand OpenListManufacturerCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListManufacturerViewModel))
        ));

        public bool Load() => _erp.Menu.RegisterMenu("data/manufacturers", "{Manufacturers}",
                OpenListManufacturerCommand,
                "icons/manufacturer");

        }    
}
