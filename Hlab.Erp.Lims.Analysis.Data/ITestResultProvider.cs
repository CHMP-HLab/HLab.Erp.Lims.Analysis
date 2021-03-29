using HLab.Erp.Conformity.Annotations;

namespace HLab.Erp.Lims.Analysis.Data
{
    public interface ITestResultProvider
    {
        IFormTarget TestProvider { get; }
        string Values { get; set; }

        string Result { get; set; }
        bool MandatoryDone { get; set; }

        string Conformity { get; set; }
        ConformityState ConformityId { get; set; }
    }
}