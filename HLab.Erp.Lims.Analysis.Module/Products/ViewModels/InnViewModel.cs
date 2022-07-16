using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;
using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;

namespace HLab.Erp.Lims.Analysis.Module.Products.ViewModels
{
    using H = H<InnViewModel>;

    public class InnViewModel: ListableEntityViewModel<Inn>
    {

        //public string SubTitle => _subTitle.Get();
        //private readonly IProperty<string> _subTitle = H.Property<string>(c => c
        //    .Set(e => e.GetSubTitle )
        //    .On(e => e.Model.Name)
        //    .Update()
        //);
        //private string GetSubTitle => $"{Model?.Variant}\n{Model?.Form?.Name}";


        public override string IconPath => _iconPath.Get();
        private readonly IProperty<string> _iconPath = H.Property<string>(c => c
        .Set(e => e.GetIconPath )
        .On(e => e.Model.IconPath)
        .Update()
        );

        private string GetIconPath => Model?.IconPath??Model?.IconPath??base.IconPath;


        private readonly Func<Inn, InnProductComponentListViewModel> _getComponents;

        public InnViewModel(Func<Inn, InnProductComponentListViewModel> getComponents)
        {
            _getComponents = getComponents;
            H.Initialize(this);
        }

        private readonly ITrigger _onEditMode = H.Trigger(c => c
            .On(e => e.Locker.IsActive)
            .Do(e =>
            {
                if(e.Components!=null)
                    e.Components.EditMode = e.Locker.IsActive;
            })
        );

        public InnProductComponentListViewModel Components => _components.Get();
        private readonly IProperty<InnProductComponentListViewModel> _components = H.Property<InnProductComponentListViewModel>(c => c
            .NotNull(e => e.Model)
            .Set(e =>
           {
               var components = e._getComponents?.Invoke(e.Model);
               //if (tests != null) tests.List.CollectionChanged += e.List_CollectionChanged;
               return components;
           })
            .On(e => e.Model)
            .Update()
        );

        public ICommand WikiCommand { get; } = H.Command(c => c.Action(e => e.Wiki()));

        private void Wiki()
        {
            var url = $"https://en.wikipedia.org/w/api.php?action=query&prop=revisions&titles={Model.Name}&rvslots=*&rvprop=content&formatversion=2&format=xml";
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(url);
            }
            catch { return; }

            XmlNode node = doc;
           // if (node is not { HasChildNodes: true }) return;

            if(!GetNode(ref node, "api","query","pages","page","revisions","rev","slots","slot")) return;

            var wiki = node.InnerText;

            Model.CasNumber = GetWikiValue(wiki, "CAS_number");
        }

        public static string GetWikiValue(string wiki, string name)
        {
            var regex = new Regex(@$"\| *{name} *=(.*?)\n");
            var result = regex.Match(wiki);
            if (result.Groups.Count <= 1) return null;

            return result.Groups[1].Value.Trim();
        }

        public static bool GetNode(ref XmlNode node, params string[] names)
        {
            foreach (var name in names)
            {
                if(!GetNode(ref node, name)) return false;
            }
            return true;
        }

        public static bool GetNode(ref XmlNode node, string name)
        {
            if (node is not { HasChildNodes: true }) return false;
            foreach (var child in node.ChildNodes.OfType<XmlNode>())
            {
                if (child.Name != name) continue;
                node = child;
                return true;
            }
            return false;
        }

        //public ProductWorkflow Workflow => _workflow.Get();
        //private readonly IProperty<ProductWorkflow> _workflow = H.Property<ProductWorkflow>(c => c
        //    .On(e => e.Model)
        //    .OnNotNull(e => e.Locker)
        //    .Set(vm => new ProductWorkflow(vm.Model,vm.Locker))
        //);
    }
    public class InnViewModelDesign : InnViewModel, IViewModelDesign
    {
        public InnViewModelDesign() : base(p => null)
        {
        }

        public new Inn Model { get; } = Inn.DesignModel;
    }
}
