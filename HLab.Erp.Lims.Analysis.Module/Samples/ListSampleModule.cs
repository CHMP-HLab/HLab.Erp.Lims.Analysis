using System.Windows.Input;
using HLab.Core.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.FormClasses;
using HLab.Erp.Lims.Analysis.Module.Products;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    using H = H<SampleListModule>;

    public class SampleListModule : NotifierBase, IBootloader
    {
        
        private readonly IErpServices _erp;

        public SampleListModule(IErpServices erp)
        {
            _erp = erp;
            H.Initialize(this);
        }


        public ICommand OpenListFormClassCommand { get; } = H.Command(c => c.Action(
            e => e._erp.Docs.OpenDocumentAsync(typeof(FormClassesListViewModel))
        ));


        public void Load(IBootContext b)
        {


                _erp.Menu.RegisterMenu("tools/formclasses", "{Form Classes}",
                    OpenListFormClassCommand,
                    "Icons/Entities/TestClass");
        }
    }
}