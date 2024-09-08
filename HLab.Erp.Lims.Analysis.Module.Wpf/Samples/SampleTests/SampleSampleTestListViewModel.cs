#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Erp.Acl;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Data.Workflows;
using HLab.Erp.Lims.Analysis.Module.SampleTests;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;
using HLab.Mvvm.Wpf;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples.SampleTests;

using H = H<SampleSampleTestListViewModel>;
public class SampleSampleTestListViewModel : Core.EntityLists.EntityListViewModel<SampleTest>, IMvvmContextProvider
{
    readonly IAclService _acl;

    public Sample Sample { get; }

    public SampleSampleTestListViewModel(IAclService acl, Injector i, Sample sample) : base(i, c => c
        //.DeleteAllowed()
        .StaticFilter(e => e.SampleId == sample.Id)

        //.Column("Expander").ContentTemplate(@$"
        //    <GroupBox xmlns:wpf=""clr-namespace:HLab.Mvvm.Wpf;assembly=HLab.Mvvm.Wpf"">
        //    <{XamlTool.Type<ViewLocator>(out var ns1)}
        //            {XamlTool.Namespace<IViewClassDocument>(out var ns2)}
        //            {XamlTool.Namespace<ViewModeDefault>(out var ns3)}
        //            ViewClass=""{{x:Type {XamlTool.Type<IViewClassDocument>(ns2)}}}""
        //            ViewMode=""{{x:Type {XamlTool.Type<ViewModeDefault>(ns3)}}}""
        //            Model=""{{Binding Model}}""/>
        //    </GroupBox>"
        //)

    //<{XamlTool.Type<ViewLocator>(ns1)}.Model>{XamlTool.ContentPlaceHolder}</{XamlTool.Type<ViewLocator>(ns1)}.Model>
    //</{XamlTool.Type<ViewLocator>(ns1)}>""

        .Column("Order").Header("{Order").Width(35).IconPath("Icons/Sort/Sort")
        .Content(s => s.Order)
        .OrderBy(s => s.Order)
        .OrderByAsc(0)

        .DescriptionColumn(s => s.TestName, s => s.Description, "Test")
        .Header("{Test}")//.Mvvm<IDescriptionViewClass>()
        .IconPath("Icons/Entities/TestClass")
        .Width(300)
        .Icon(s => s.IconPath)
        .OrderBy(s => s.Order).UpdateOn(s => s.TestName).UpdateOn(s => s.Description)

        .DescriptionColumn(s => "", s => s.Specification, "Specification")
        .Header("{Specifications}")
        .IconPath("Icons/Workflows/Specifications")
        .Width(200)
        .OrderBy(s => s.Specification).UpdateOn(s => s.Specification)

        .DescriptionColumn(s => "", s => s.Result.Result, "Result")
        .Header("{Result}")
        .IconPath("Icons/Workflows/Certificate")
        .Width(200)
        .OrderBy(s => s.Result.Result)
        .UpdateOn(s => s.Result.Result)

        .DescriptionColumn(s => "", s => s.Result.Conformity, "Conformity")
        .Header("{Conformity}")
        .Width(200)
        .OrderBy(s => s.Result?.Conformity ?? "")
        .UpdateOn(s => s.Result.Conformity)

        .ConformityColumn(s => s.Result != null ? s.Result.ConformityId : ConformityState.NotChecked)
        .UpdateOn(s => s.Result.ConformityId)

        .StageColumn(default(SampleTestWorkflow), s => s.StageId)

        .AddProperty("IsValid", s => s?.Stage != null, s => s.Stage != SampleTestWorkflow.InvalidatedResults)
        .AddProperty("Group", s => s?.TestClassId != null, s => s.TestClassId)

    )
    {
        var n = SampleTestWorkflow.Specifications; // HACK : this is a hack to force top level static constructor

        _acl = acl;
        Sample = sample;

        H.Initialize(this);

        ShowFilters = false;
        // List.AddOnCreate(h => h.Entity. = "<Nouveau Critère>").Update();
    }

    public bool EditMode { get => _editMode.Get(); set => _editMode.Set(value); }
    readonly IProperty<bool> _editMode = H.Property<bool>(c => c.Default(false));

    protected override bool CanExecuteDelete(SampleTest sampleTest, Action<string> errorAction)
    {
        if (!EditMode) return false;
        if (sampleTest == null) return false;
        var stage = sampleTest.Stage.IsAny(errorAction, SampleTestWorkflow.Specifications);
        var granted = _acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest);
        return stage && granted;
    }

    public override Type AddArgumentClass => typeof(TestClass);

    readonly ITrigger _1 = H.Trigger(c => c
        .On(e => e.Sample.Stage)
        .On(e => e.Sample.Pharmacopoeia)
        .On(e => e.Sample.PharmacopoeiaVersion)
        .On(e => e.EditMode)
        .Do(e => (e.AddCommand as CommandPropertyHolder)?.CheckCanExecute())
    );

    readonly ITrigger _2 = H.Trigger(c => c
        .On(e => e.List.Item().Result.ConformityId)
        .Do(e => e.UpdateConformity())
    );

    //private readonly ITrigger _triggerConformity = H.Trigger(c => c
    //    .On(e => e.List.Item().Result.ConformityId)
    //    .On(e => e.List.Item().Result.Stage)
    //    .On(e => e.List.Item().Result.Start)
    //    .On(e => e.List.Item().Result.End)
    //    .Do(e => e.List.Refresh())
    //);

    protected override bool CanExecuteAdd(Action<string> errorAction)
    {
        AllowManualOrder = false;

        if (!EditMode) return false;
        if (!_acl.IsGranted(errorAction, AnalysisRights.AnalysisAddTest)) return false;
        if (Sample.Pharmacopoeia == null)
        {
            errorAction("{Missing} : {Pharmacopoeia}");
            return false;
        }
        if (string.IsNullOrWhiteSpace(Sample.PharmacopoeiaVersion))
        {
            errorAction("{Missing} : {Pharmacopoeia version}");
            return false;
        }
        if (!Sample.Stage.IsAny(errorAction, SampleWorkflow.Monograph))
        {
            return false;
        }
        AllowManualOrder = true;
        return true;
    }

    protected override Task ConfigureNewEntityAsync(SampleTest st, object arg)
    {
        if (arg is not TestClass testClass) return Task.CompletedTask;

        st.Sample = Sample;
        st.TestClass = testClass;
        st.Pharmacopoeia = Sample.Pharmacopoeia;
        st.PharmacopoeiaVersion = Sample.PharmacopoeiaVersion;
        //st.Code = testClass.Code;
        st.Description = "";
        st.TestName = testClass.Name;
        st.Stage = SampleTestWorkflow.DefaultStage;

        return Task.CompletedTask;
    }

    public void UpdateConformity()
    {
        if(Sample==null) return;
        if (Filters.Count != 1) return;

        var conformity = ConformityState.None;

        foreach (var sampleTest in List)
        {
            conformity = UpdateConformity(conformity, sampleTest.Result?.ConformityId ?? ConformityState.NotChecked);
        }

        if (Sample.ConformityId == conformity) return;

        Injected.Data.UpdateAsync(Sample, s => s.ConformityId = conformity);
    }

    static ConformityState UpdateConformity(ConformityState currentState, ConformityState testState)
    {
        switch (testState)
        {
            case ConformityState.NotChecked:
                return currentState switch
                {
                    ConformityState.None => ConformityState.NotChecked,
                    ConformityState.NotChecked => ConformityState.NotChecked,
                    ConformityState.Running => ConformityState.Running,
                    ConformityState.NotConform => ConformityState.NotConform,
                    ConformityState.Conform => ConformityState.Running,
                    ConformityState.Invalid => ConformityState.Invalid,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                };
            case ConformityState.Running:
                return currentState switch
                {
                    ConformityState.None => ConformityState.Running,
                    ConformityState.NotChecked => ConformityState.Running,
                    ConformityState.Running => ConformityState.Running,
                    ConformityState.Conform => ConformityState.Running,
                    ConformityState.NotConform => ConformityState.NotConform,
                    ConformityState.Invalid => ConformityState.Invalid,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                };
            case ConformityState.NotConform:
                return currentState switch
                {
                    ConformityState.None => ConformityState.NotConform,
                    ConformityState.NotChecked => ConformityState.NotConform,
                    ConformityState.Running => ConformityState.NotConform,
                    ConformityState.Conform => ConformityState.NotConform,
                    ConformityState.NotConform => ConformityState.NotConform,
                    ConformityState.Invalid => ConformityState.NotConform,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                };
            case ConformityState.Conform:
                return currentState switch
                {
                    ConformityState.None => ConformityState.Conform,
                    ConformityState.NotChecked => ConformityState.Running,
                    ConformityState.Running => ConformityState.Running,
                    ConformityState.Conform => ConformityState.Conform,
                    ConformityState.NotConform => ConformityState.NotConform,
                    ConformityState.Invalid => ConformityState.Invalid,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                };
            case ConformityState.Invalid:
                return currentState switch
                {
                    ConformityState.None => ConformityState.Invalid,
                    ConformityState.NotChecked => ConformityState.Invalid,
                    ConformityState.Running => ConformityState.Invalid,
                    ConformityState.Conform => ConformityState.Invalid,
                    ConformityState.Invalid => ConformityState.Invalid,
                    ConformityState.NotConform => ConformityState.NotConform,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
                };
            default:
                throw new ArgumentOutOfRangeException();
        }

    }


    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }
}