using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    using H = NotifyHelper<ListSampleModule>;
    public class ListSampleModule : IBootloader //postboot
    {
        
        [Import] private readonly IErpServices _erp;

        [Import] public ListSampleModule()
        {
            H.Initialize(this);

        }

        public ICommand OpenListSampleCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListSampleViewModel))
        ));
        public ICommand OpenListTestClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListTestClassViewModel))
        ));
        public ICommand OpenListTestCategoryCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListTestCategoryViewModel))
        ));


        int step = 0;
        public bool Load()
        {
            if(step<1)
            {
                if(!_erp.Menu.RegisterMenu("data/samples", "{Samples}",
                    OpenListSampleCommand,
                    "icons/Entities/Sample")) return false;
                step = 1;
            }
            if(step<2)
            {
                if(!_erp.Menu.RegisterMenu("tools/testclasses", "{Test Classes}",
                    OpenListTestClassCommand,
                    "Icons/Entities/TestClass"))return false;
                step = 2;
            }
            if(step<3)
            {
                if(!_erp.Menu.RegisterMenu("tools/testcategories", "{Test Categories}",
                    OpenListTestCategoryCommand,
                    "Icons/Entities/TestCategory"))return false;
            }
            return true;
        }
    }
}