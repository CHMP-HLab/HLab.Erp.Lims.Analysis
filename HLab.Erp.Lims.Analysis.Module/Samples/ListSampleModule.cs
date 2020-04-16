using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.Products;
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
            e => e._erp.Docs.OpenDocumentAsync(typeof(SamplesListViewModel))
        ));
        public ICommand OpenListTestClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListTestClassViewModel))
        ));
        public ICommand OpenListTestCategoryCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ProductCategoriesListViewModel))
        ));


        public void Load(IBootContext b)
        {
                _erp.Menu.RegisterMenu("data/samples", "{Samples}",
                    OpenListSampleCommand,
                    "Icons/Entities/Sample");
 
                _erp.Menu.RegisterMenu("tools/testclasses", "{Test Classes}",
                    OpenListTestClassCommand,
                    "Icons/Entities/TestClass");

                _erp.Menu.RegisterMenu("tools/testcategories", "{Test Categories}",
                    OpenListTestCategoryCommand,
                    "Icons/Entities/TestCategory");
        }
    }
}