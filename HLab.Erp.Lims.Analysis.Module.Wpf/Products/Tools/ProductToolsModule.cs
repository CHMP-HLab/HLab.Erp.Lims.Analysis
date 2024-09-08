using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Products.Tools;

using H = H<ProductToolsModule>;

public class ProductToolsModule : NotifierBase, IBootloader
{
    readonly IDocumentService _docs;
    readonly IAclService _acl;
    readonly IMenuService _menu;

    public ProductToolsModule(IDocumentService docs, IAclService acl, IMenuService menu)
    {
        _docs = docs;
        _acl = acl;
        _menu = menu;
        H.Initialize(this);
    }

    public ICommand OpenCommand { get; } = H.Command(c => c.Action(
        e => e._docs.OpenDocumentAsync(typeof(ProductToolsViewModel))
    ).CanExecute(e => true));

    protected virtual string IconPath => "Icons/Entities/";

    public virtual void Load(IBootContext b)
    {
        if (b.WaitDependency("BootLoaderErpWpf")) return;

        if (_acl.Connection == null)
        {
            if(!_acl.Cancelled) b.Requeue();
            return;
        }

        if(!_acl.IsGranted(AclRights.ManageUser)) return;

        _menu.RegisterMenu("tools/ProductTools", "{Product Tools}",
            OpenCommand,
            "icons/tools/ProductTools");
    }
}