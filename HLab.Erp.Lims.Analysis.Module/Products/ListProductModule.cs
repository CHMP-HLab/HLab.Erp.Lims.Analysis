using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products
{
    using H = NotifyHelper<ListProductModule>;

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
            _erp.Menu.RegisterMenu("data", "products", "{Products}",
                OpenListProductCommand,
                "icons/sample/drugs");

        }
    }
}
