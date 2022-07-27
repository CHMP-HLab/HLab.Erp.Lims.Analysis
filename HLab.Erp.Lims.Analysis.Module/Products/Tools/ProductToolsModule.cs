using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.Products.Tools;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Acl.Users
{
    using H = H<ProductToolsModule>;

    public class ProductToolsModule : NotifierBase, IBootloader
    {
        readonly IErpServices _erp;

        public ProductToolsModule(IErpServices erp)
        {
            _erp = erp; 
            H.Initialize(this);
        }

        public ICommand OpenCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ProductToolsViewModel))
        ).CanExecute(e => true));

        protected virtual string IconPath => "Icons/Entities/";

        public virtual void Load(IBootContext b)
        {
            if (b.WaitDependency("BootLoaderErpWpf")) return;

            if (_erp.Acl.Connection == null)
            {
                if(!_erp.Acl.Cancelled) b.Requeue();
                return;
            }

            if(!_erp.Acl.IsGranted(AclRights.ManageUser)) return;

            _erp.Menu.RegisterMenu("tools/ProductTools", "{Product Tools}",
                OpenCommand,
                "icons/tools/ProductTools");
        }
    }
}
