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
        public object Form
        {
            get => _form.Get();
            private set => _form.Set(value);
        }
        private readonly IProperty<object> _form = H.Property<object>();
        public async Task Compile(SampleTestResult target)
        {
            if (target == null)
            {
                Form = null;
                return;
            }

            var h = new FormHelper();
            await h.ExtractCode(target.SampleTest.Code).ConfigureAwait(true);
            await h.LoadForm().ConfigureAwait(true);

            h.LoadValues(target.SampleTest.Values);
            h.LoadValues(target.Values);

            Form = h.Form;
        }

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

            SelectAction = async t => await Compile(t);

            List.AddFilter(() => e => e.SampleTestId == sampleTestId);


            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s => s.Result)
                .Icon("{State}", s => s.StateId != null ? GetStateIcon(s.StateId.Value) : "", s => s.StateId)
;
            //List.AddFilter(e => e.State < 3);

            // Db.Fetch<Customer>();
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
