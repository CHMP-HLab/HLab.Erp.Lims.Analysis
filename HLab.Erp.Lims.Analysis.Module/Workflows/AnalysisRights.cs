using HLab.Erp.Acl;

namespace HLab.Erp.Lims.Analysis.Module.Workflows
{
    public static class AnalysisRights
    {
        //Reception
        public static readonly AclRight AnalysisReceptionValidate = AclRight.Get();

        //Analysis
        public static readonly AclRight AnalysisSchedule = AclRight.Get();

        //Tests
        public static readonly AclRight AnalysisAddTest = AclRight.Get();
        public static readonly AclRight AnalysisAddResult = AclRight.Get();

        //Results
        public static readonly AclRight AnalysisResultValidate = AclRight.Get();
        public static readonly AclRight AnalysisResultEnter = AclRight.Get();
        public static readonly AclRight AnalysisResultCheck = AclRight.Get();



        //Certificate
        public static readonly AclRight AnalysisCertificateCreate = AclRight.Get();

    }
}