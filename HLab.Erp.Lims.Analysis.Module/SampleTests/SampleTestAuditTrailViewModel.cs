using HLab.Erp.Acl;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Lims.Analysis.Data.Workflows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class SampleTestAuditTrailViewModel : Core.EntityLists.EntityListViewModel<AuditTrail>
    {
        static string GetStage(string log)
        {
            var lines = log.Replace("\r","").Split('\n');
            foreach (var line in lines)
            {
                var part = line.Split('=');

                if(part.Length>1)
                {
                    switch(part[0])
                    {
                        case "Stage":
                        case "StageId":
                        return SampleTestWorkflow.StageFromName(part[1]).GetCaption(null);
                    }
                }
            }
            return "NA";
        }

        public SampleTestAuditTrailViewModel(Injector i, int sampleTestId) : base(i, c => c
            .StaticFilter(e => e.EntityId == sampleTestId)
            .StaticFilter(e => e.EntityClass == "SampleTest")

             .Column("Stage")
            .Header("{Stage}").Width(150).Content(at => $"{GetStage(at.Log)}").Localize()
            .Icon(at => at.IconPath,20)

            .Column("Date")
            .Header("{Date}").Width(110).Content(at => at.TimeStamp)
            
             .Column("User")
            .Header("{User}").Width(150).Content(at => at.UserCaption)
           
            .Column("Motivation")
            .Header("{Motivation}").Width(250)
            .Content(at => at.Motivation)

             .Column("Log")
            .Header("{Log}").Width(150).Content(at => $"{at.Log}").Localize()
        )
        {
        }

        string LogAbstract(string log, int size)
        {
            const string suffix = "...";

            var result = log.Replace('\n', '/').Replace("\r","");
            if (result.Length < size) return result;
            result = result.Substring(0, Math.Max(0,size - suffix.Length)) + suffix;
            return result;
        }
    }
}
