using System;

using HLab.Erp.Acl;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleAuditTrailViewModel : EntityListViewModel<AuditTrail>
    {
        public SampleAuditTrailViewModel(int sampleId) : base(c => c
            .StaticFilter(e => e.EntityId == sampleId)
            .StaticFilter(e => e.EntityClass == "Sample")
            .StaticFilter(e => e.Motivation != null)

            .Column()
            .Header("{Date}").Width(110).Content(at => at.TimeStamp)
            
            .Column()
            .Header("{Icon}").Width(80)
            .Icon(at => at.IconPath)
            
            .Column()
            .Header("{Motivation}").Width(250)
            .Content(at => at.Motivation)
            
            .Column()
            .Header("{User}").Width(150).Content(at => at.UserCaption)
        )
        {
        }

        private string LogAbstract(string log, int size)
        {
            const string suffix = "...";

            var result = log.Replace('\n', '/').Replace("\r","");
            if (result.Length < size) return result;
            result = result.Substring(0, Math.Max(0,size - suffix.Length)) + suffix;
            return result;
        }

    }
}
