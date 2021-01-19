﻿using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class ListSampleTestViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
    {
        [Import] private readonly IAclService _acl;

        private string GetIcon(int? state)
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
            var state = SampleTestWorkflow.StateFromName(name);
            return state?.GetIconPath(null);
        }
        private string GetStateCaption(string name)
        {
            var state = SampleTestWorkflow.StateFromName(name);
            return state?.GetCaption(null);
        }

        public ListSampleTestViewModel(int sampleId)
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

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
                .Column("{Result}", s => s.Result?.Result??"", s => s.Result?.Result??"")
                .Icon("Conformity", s => GetIcon(s.Result?.StateId), s => s.Result?.StateId)
                .Icon("{Stage}", s => GetStateIcon(s.Stage), s => s.Stage)
                .Localize("{Stage}", s=>GetStateCaption(s.Stage), s=>s.Stage)
                .Hidden("IsValid", s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Hidden("Group", s => s.TestClassId);

            List.UpdateAsync();

            DeleteAllowed = true;
        }

        protected override bool CanExecuteDelete()
        {
            if(Selected==null) return false;
            if (Selected.Stage != SampleTestWorkflow.Specifications.Name) return false;
            if(!_acl.IsGranted(AnalysisRights.AnalysisAddTest)) return false;
            return true;
        }

        public override string Title => "Samples";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }

}
