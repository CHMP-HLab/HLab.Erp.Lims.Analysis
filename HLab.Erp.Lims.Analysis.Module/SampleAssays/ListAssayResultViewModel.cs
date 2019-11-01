using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.AssayClasses;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.SampleAssays
{
    public class ListAssayResultViewModel : EntityListViewModel<ListAssayResultViewModel,AssayResult>, IMvvmContextProvider
    {
        [Import]
        private readonly IErpServices _erp;
        public object Form
        {
            get => _form.Get();
            private set => _form.Set(value);
        }
        private readonly IProperty<object> _form = H.Property<object>();
        public async Task Compile(AssayResult target)
        {
            if (target == null)
            {
                Form = null;
                return;
            }

            var h = new FormHelper();
            await h.ExtractCode(target.SampleAssay.Code).ConfigureAwait(true);
            Form = await h.LoadForm(target.Values).ConfigureAwait(true);
            //XamlMessage = h.XamlMessage;
            //CsMessage = h.CsMessage;
        }

        private async Task<object> GetStateIcon(int state)
        {
            switch(state)
            {
                case 1:
                    return await _erp.Icon.GetIcon("icons/Results/CheckFailed");
                case 2:
                    return await _erp.Icon.GetIcon("icons/Results/GaugeKO");
                case 3:
                    return await _erp.Icon.GetIcon("icons/Results/GaugeOK");
                default:
                    return await _erp.Icon.GetIcon("icons/Results/Gauge");
            }
        }

        public ListAssayResultViewModel(int sampleAssayId)
        {
            OpenAction = t => Compile(t);

            List.AddFilter(()=>e => e.SampleAssayId == sampleAssayId);


            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("", s=> s.Result)
                .Column("^State",  async s => s.StateId != null ? await GetStateIcon(s.StateId.Value) : "")
;
            //List.AddFilter(e => e.State < 3);

            // Db.Fetch<Customer>();
            using (List.Suspender.Get())
            {

            }

        }

        public string Title => "Sample";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
