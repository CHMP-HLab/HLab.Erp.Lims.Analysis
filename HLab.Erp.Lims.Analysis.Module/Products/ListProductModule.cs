using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Lims.Analysis.Module.Products.ListProductModule>;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    public class ListProductModule : IPostBootloader
    {
        private readonly IErpServices _erp;

        [Import] public ListProductModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }
        public ICommand OpenListProductCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListProductViewModel))
        ));

        public void Load()
        {
            _erp.Menu.RegisterMenu("data", "products", "Products",
                OpenListProductCommand,
                _erp.Icon.GetIcon("icons/sample/drugs"));

        }
    }
}
