using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.TestClasses;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class ListTestResultViewModel : EntityListViewModel<ListTestResultViewModel, SampleTestResult>, IMvvmContextProvider
    {
        [Import]
        private readonly IErpServices _erp;

        private string GetStateIcon(int state)
        {
            switch (state)
            {
                case 1:
                    return "icons/Results/CheckFailed";
                case 2:
                    return "icons/Results/GaugeKO";
                case 3:
                    return "icons/Results/GaugeOK";
                default:
                    return "icons/Results/Gauge";
            }
        }

        private int _sampleTestId;

        public ListTestResultViewModel(int sampleTestId)
        {
            _sampleTestId = sampleTestId;

            AddAllowed=false;
            DeleteAllowed=false;

            List.AddFilter(() => e => e.SampleTestId == sampleTestId);

            List.OrderBy = e => e.Start;

             Columns
                .Column("{Start}", s => s.Start)
                .Column("{End}", s => s.End)
                .Column("{Result}", s => s.Result)
                .Icon("{State}", s => s.StateId != null ? GetStateIcon(s.StateId.Value) : "", s => s.StateId)
                .Icon("{Validation}", s => s.Validation != null ? GetStateIcon(s.Validation.Value) : "", s => s.Validation)
                .Hidden("IsSelected",s => s.Id == s.SampleTest.Result?.Id)
;

            using (List.Suspender.Get())
            {

            }

        }

        [Import] private IDataService _data;

        protected override async Task OnAddCommand(SampleTestResult target)
        {
            var result = _data.Add<SampleTestResult>(r =>
            {
                r.SampleTestId = _sampleTestId;
                if(target!=null)
                {
                    
                }
            });
            if(result!=null)
                List.Update();

        }

        public string Title => "{Result}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
