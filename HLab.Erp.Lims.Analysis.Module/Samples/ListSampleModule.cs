using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Erp.Lims.Analysis.Module.ListSampleModule>;

namespace HLab.Erp.Lims.Analysis.Module
{
    public class ListSampleModule : IPostBootloader
    {
        
        [Import] private readonly IErpServices _erp;

        [Import] public ListSampleModule()
        {
            H.Initialize(this);

        }

        public ICommand OpenListSampleCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListSampleViewModel))
        ));
        public ICommand OpenListTestClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListTestClassViewModel))
        ));
        public ICommand OpenListTestCategoryCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocument(typeof(ListTestCategoryViewModel))
        ));

        public void Load()
        {
            _erp.Menu.RegisterMenu("data", "samples", "{Samples}",
                OpenListSampleCommand,
                _erp.Icon.GetIconAsync("icons/Entities/Sample"));

            _erp.Menu.RegisterMenu("tools", "testclasses", "{Test Classes}",
                OpenListTestClassCommand,
                _erp.Icon.GetIconAsync("Icons/Entities/TestClass"));

            _erp.Menu.RegisterMenu("tools", "testcategories", "{Test Categories}",
                OpenListTestCategoryCommand,
                _erp.Icon.GetIconAsync("Icons/Entities/TestCategory"));
        }
    }
}