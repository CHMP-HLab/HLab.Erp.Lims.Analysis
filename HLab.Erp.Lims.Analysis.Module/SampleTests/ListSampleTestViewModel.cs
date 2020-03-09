using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ViewModels;
using HLab.Erp.Core.ViewModels.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class ListSampleTestViewModel : EntityListViewModel<ListSampleTestViewModel, SampleTest>, IMvvmContextProvider
    {
        [Import]
        private readonly IIconService _icons;

        private string GetIcon(int state)
        {
            switch(state)
            {
                case 1:
                    return "icons/Results/Gauge";
                case 2:
                    return 
                        "icons/Results/GaugeKo";
                case 3:
                    return "icons/Results/GaugeOk";
                default:
                    return "icons/Results/Gauge";
            }
        }
        private string GetCheckIcon(int state)
        {
            switch (state)
            {
                case 1:
                    return "icons/Results/Running";
                case 2:
                    return "icons/Results/CheckFailed";
                case 3:
                    return "icons/Results/CheckPassed";
                default:
                    return "icons/Results/Running";
            }
        }
        private string GetStateIcon(string name)
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            var state = SampleTestWorkflow.StateFromName(name);
            return state?.GetIconPath(null);
        }
        private string GetStateCaption(string name)
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            var state = SampleTestWorkflow.StateFromName(name);
            return state?.GetCaption(null);
        }

        public ListSampleTestViewModel(int sampleId)
        {
            List.AddFilter(()=>e => e.SampleId == sampleId);
            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns
                .Icon("", s => s.TestClass.IconPath, s => s.TestClass.Order)
                .Column("{Test}", s => new StackPanel{
                    VerticalAlignment = VerticalAlignment.Top,
                    Children =
                    {
                        new TextBlock{Text=s.TestName,FontWeight = FontWeights.Bold},
                        new TextBlock{Text = s.Description, FontStyle = FontStyles.Italic}
                    }})
                .Column("{Specifications}", s => s.Specification)
                .Column("{Result}", s => s.Result?.Result??"", s => s.Result)
            //.Column("Conformity", s => s.TestStateId);
                .Icon("{Stage}", s => GetStateIcon(s.Stage), s => s.Stage)
                .Localize("{Stage}", s=>GetStateCaption(s.Stage), s=>s.Stage)
                .Hidden("IsValid", s => s.Validation!=2)
                .Hidden("Group", s => s.TestClassId);

            List.UpdateAsync();
        }

        public string Title => "Samples";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
