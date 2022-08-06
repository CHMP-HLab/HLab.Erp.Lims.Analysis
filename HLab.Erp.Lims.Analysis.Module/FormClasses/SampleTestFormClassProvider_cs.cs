using System.Threading.Tasks;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

public partial class SampleTestFormClassProvider
{
    const string CsHeader = 
        "using System.Runtime;"
        + "using System.ComponentModel;"
        + "using System.Text;"
        + "using HLab.Erp.Lims.Analysis.Module.FormClasses;"
        + "using HLab.Notify.PropertyChanged;"
        + "using HLab.Notify.Annotations;" 
        + "using HLab.Erp.Conformity.Annotations;"
        + "using HLab.Notify.Wpf;" 
        + "using HLab.Base.Wpf;"
        + "using HLab.Erp.Lims.Analysis.Module.TestClasses;"
        + "using Outils;" 
        + "/*Content*/";

    protected override async Task<string> PrepareCsAsync(string cs)
    {
        cs = CsHeader.Replace("/*Content*/", cs);

        if (cs.Contains("using FM;"))
        {
            cs = cs.Replace("using FM;", "");
        }
        if (cs.Contains("void Traitement("))
        {
            cs = cs.Replace("void Traitement(", "void Process(");
        }

        return await base.PrepareCsAsync(cs);
    }


}