using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleAuditTrailViewModel : EntityListViewModel<AuditTrail>
    {
                public SampleAuditTrailViewModel(int sampleId)
        {
            List.AddFilter(() => e => e.EntityId == sampleId);
            List.AddFilter(() => e => e.EntityClass == "Sample");
            List.AddFilter(() => e => e.Motivation != null);

            DeleteAllowed = false;
            AddAllowed = false;

            Columns
                .Column("{Date}", at => at.TimeStamp)
//                .Column("{Action}", at => at.Action)
//                .Column("{Caption}", at=>at.EntityCaption)
//                .Column("{Class}", at=>at.EntityClass)
                .Icon("{Icon}", at=>at.IconPath)
//                .Column("{Log}", at=>LogAbstract(at.Log,50))
                .Column("{Motivation}", at=>at.Motivation)
                .Column("{User}", at=>at.UserCaption)
                ;

            //AddFilter<FilterDateViewModel>(f =>  f
            //    .Title("{Date}")
            //    .MaxDate(DateTime.Now)
            //    .MinDate(DateTime.Now - TimeSpan.FromDays(30))
            //    .Link(this,a => a.TimeStamp)
            //);

            //AddFilter<FilterTextViewModel>(f =>  f
            //    .Title("{Action}")
            //    .Link(this,a => a.Action)
            //);

            //AddFilter<FilterTextViewModel>(f => f
            //    .Title("{Caption}")
            //    .Link(this,at=>at.EntityCaption)
            //);

            //AddFilter<FilterTextViewModel>(f => f
            //    .Title("{Class}")
            //    .Link(this,at=>at.EntityClass)
            //);

            List.UpdateAsync();
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
