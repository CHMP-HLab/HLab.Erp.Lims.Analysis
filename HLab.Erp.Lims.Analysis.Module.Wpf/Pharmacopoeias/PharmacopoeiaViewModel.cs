using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.Pharmacopoeias;

public class PharmacopoeiaViewModelDesign : PharmacopoeiaViewModel, IViewModelDesign
{


    public PharmacopoeiaViewModelDesign():base(null)
    {
        Model = Pharmacopoeia.DesignModel;
    }
}
public class PharmacopoeiaViewModel : ListableEntityViewModel<Pharmacopoeia>
{
    public PharmacopoeiaViewModel(Injector i) : base(i)
    {
    }
}