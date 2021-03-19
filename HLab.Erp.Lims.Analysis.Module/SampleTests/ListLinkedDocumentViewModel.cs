using System;
using System.Threading.Tasks;

using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class ListLinkedDocumentViewModel : EntityListViewModel<LinkedDocument>, IMvvmContextProvider
    {
        [Import]
        private readonly IErpServices _erp;

        private readonly int _sampleTestId;

        public ListLinkedDocumentViewModel(int sampleTestId)
        {
            _sampleTestId = sampleTestId;

            AddAllowed=false;
            DeleteAllowed=false;

            List.AddFilter(() => e => e.SampleTestResultId == sampleTestId);

            //List.OrderBy = e => e.Start;

             Columns.Configure(c => c
                    .Column
                    .Header("{Name}").Width(200)
                    .Content(s => s.Name)
             );

            using (List.Suspender.Get())
            {
                DeleteAllowed = true;
            }

        }

        protected override async Task AddEntityAsync()
        {
            var target = Selected;

            int i = 0;

            foreach (var r in List)
            {
                var n = r.Name;
                if (n.StartsWith("R")) n = n.Substring(1);

                if(int.TryParse(n, out var v))
                {
                    i = Math.Max(i,v);
                }
            }


            var result  = await _erp.Data.AddAsync<SampleTestResult>(r =>
            {
                r.Name = $"R{i + 1}";
                r.SampleTestId = _sampleTestId;
                if(target!=null)
                {
                    
                }
            });
            if(result!=null)
                List.Update();

        }
        protected override bool CanExecuteDelete() => Selected != null;

        public override string Title => "{Documents}";

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}
