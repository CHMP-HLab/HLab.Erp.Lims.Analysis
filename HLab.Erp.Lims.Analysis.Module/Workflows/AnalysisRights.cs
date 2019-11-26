using HLab.Erp.Acl;

namespace HLab.Erp.Lims.Analysis.Module.Workflows
{
    public static class AnalysisRights
    {
        public static readonly AclRight AnalysisCertificateCreate = AclRight.Get();
        public static readonly AclRight AnalysisResultValidate = AclRight.Get();
        public static readonly AclRight AnalysisResultEnter = AclRight.Get();
        public static readonly AclRight AnalysisSchedule = AclRight.Get();
        public static readonly AclRight AnalysisAddTest = AclRight.Get();
        public static readonly AclRight AnalysisAddResult = AclRight.Get();
    }
}