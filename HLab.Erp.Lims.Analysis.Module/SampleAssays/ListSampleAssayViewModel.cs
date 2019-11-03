using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;

namespace HLab.Erp.Lims.Analysis.Module.SampleAssays
{
    public class ListSampleAssayViewModel : EntityListViewModel<ListSampleAssayViewModel, SampleAssay>, IMvvmContextProvider
    {
        [Import]
        private readonly IIconService _icons;

        private async Task<object> GetIcon(int state,double size)
        {
            switch(state)
            {
                case 1:
                    return await _icons.GetIcon("icons/Results/Gauge", size).ConfigureAwait(false);
                case 2:
                    return await _icons.GetIcon("icons/Results/GaugeKo",size).ConfigureAwait(false);
                case 3:
                    return await _icons.GetIcon("icons/Results/GaugeOk",size).ConfigureAwait(false);
                default:
                    return await _icons.GetIcon("icons/Results/Gauge",size).ConfigureAwait(false);
            }
        }
        private async Task<object> GetCheckIcon(int state,double size)
        {
            switch (state)
            {
                case 1:
                    return await _icons.GetIcon("icons/Results/Running",size).ConfigureAwait(false);
                case 2:
                    return await _icons.GetIcon("icons/Results/CheckFailed",size).ConfigureAwait(false);
                case 3:
                    return await _icons.GetIcon("icons/Results/CheckPassed",size).ConfigureAwait(false);
                default:
                    return await _icons.GetIcon("icons/Results/Running",size).ConfigureAwait(false);
            }
        }

        public ListSampleAssayViewModel(int sampleId)
        {
            List.AddFilter(()=>e => e.SampleId == sampleId);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Column("",s=>_icons.GetIcon(s.AssayClass.IconPath, 25.0))
                .Column("^Assay", s => new StackPanel{
                    VerticalAlignment = VerticalAlignment.Top,
                    Children =
                    {
                        new TextBlock{Text=s.AssayName,FontWeight = FontWeights.Bold},
                        new TextBlock{Text = s.Description, FontStyle = FontStyles.Italic}
                    }})
                .Column("^Specifications", s => s.Specification)
                .Column("^Result", s => s.Result)
            //.Column("Conformity", s => s.AssayStateId);
                .Column("^State", async s => await GetIcon(s.AssayStateId??0,25), s => s.AssayStateId)
                .Column("^Validation", async s => await GetCheckIcon(s.Validation??0,25), s => s.Validation)
                .Hidden("IsValid", s => s.Validation!=2)
                .Hidden("Group", s => s.AssayClassId);

            List.Update();
        }

        public string Title => "Samples";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
