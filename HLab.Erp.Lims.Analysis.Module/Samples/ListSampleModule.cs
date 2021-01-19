using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    using H = H<ListSampleModule>;

    public class ListSampleModule : NotifierBase, IBootloader //postboot
    {
        
        [Import] private readonly IErpServices _erp;

        public ListSampleModule() => H.Initialize(this);


        public ICommand OpenListSampleCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(SamplesListViewModel))
        ));
        public ICommand OpenListTestClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListTestClassViewModel))
        ));
        public ICommand OpenListTestCategoryCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ProductCategoriesListViewModel))
        ));
        public ICommand OpenListFormClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(ListFormClassViewModel))
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

                _erp.Menu.RegisterMenu("tools/formclasses", "{Form Classes}",
                    OpenListFormClassCommand,
                    "Icons/Entities/TestClass");
        }
    }
}