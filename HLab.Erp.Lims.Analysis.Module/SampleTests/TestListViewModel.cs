﻿using System.Windows;
using System.Windows.Controls;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Mvvm.Annotations;
using Outils;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public class TestListViewModel : EntityListViewModel<SampleTest>, IMvvmContextProvider
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

        public TestListViewModel()
        {
            var n = SampleTestWorkflow.Specifications; // TODO : this is a hack to force top level static constructor

            // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
            Columns.Configure(c => c
                .Column
                    .Header("{Sample}")
                    .Content(s => s.Sample.Reference)

                .Column
                    .Icon(s => s.Sample.Product.IconPath)
                    .OrderBy( s => s.Sample.Product.Caption)

                .Column
                    .Header("{Product}")
                    .Content(s => s.Sample.Product.Caption)
                
                .Column
                    .Header("{Test}").Mvvm<IDescriptionViewClass>()
                    .OrderBy(s => s.Order)

                .Column
                    .Header("{Specifications}")
                    .Content(s => new StackPanel{
                        VerticalAlignment = VerticalAlignment.Top,
                        Children =
                        {
                            new TextBlock{},
                            new TextBlock{Text = Print.Langue(s.Specification,"FR"), FontStyle = FontStyles.Italic}
                        }})
                    .OrderBy(s => s.Specification)

                .Column.Header("{Result}").Content(s => new StackPanel{
                    VerticalAlignment = VerticalAlignment.Top,
                    Children =
                    {
                        new TextBlock{},
                        new TextBlock{Text = Print.Langue(s.Result?.Result??"","FR"), FontStyle = FontStyles.Italic}
                    }}).OrderBy(s => s.Result?.Result??"")
                //.Column("{Specifications}", s => s.Specification)
                //.Column("{Result}", s => s.Result?.Result??"")
                .Column.Header("{Conformity}").Icon(s => GetIcon(s.Result?.StateId)).OrderBy(s => s.Result?.StateId)
                .StageColumn(s => SampleTestWorkflow.StageFromName(s.Stage))
                .Column.Hidden.Header("IsValid").Content(s => s.Stage != SampleTestWorkflow.InvalidatedResults.Name)
                .Column.Hidden.Header("Group").Content(s => s.TestClassId)
            );
            using (List.Suspender.Get())
            {

                Filter<EntityFilter<Sample>>()
                    .Link(List, e => e.SampleId);

                Filter<EntityFilter<TestClass>>().Title("{Test Class}")
                    .Link(List, e => e.TestClassId);

                Filter<WorkflowFilterViewModel<SampleTestWorkflow>>().Title("{Stage}").IconPath("Icons/Workflow")
                    .Link(List, e => e.Stage);
            }

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

        public override string Title => "{Tests}";
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
    }
}