using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias
{
    public class PharmacopoeiaViewModelDesign : PharmacopoeiaViewModel, IViewModelDesign
    {


        public PharmacopoeiaViewModelDesign()
        {
            Model = Pharmacopoeia.DesignModel;
        }
    }
    public class PharmacopoeiaViewModel : ListableEntityViewModel<Pharmacopoeia>
    {

    }
}
