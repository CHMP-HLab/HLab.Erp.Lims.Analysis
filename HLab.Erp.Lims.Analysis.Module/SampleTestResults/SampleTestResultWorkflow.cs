using System;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Samples;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.SampleTestResults
{
    public class SampleTestResultWorkflow : Workflow<SampleTestResultWorkflow, SampleTestResult>
    {
        public SampleTestResultWorkflow(SampleTestResult result,IDataLocker locker):base(result,locker)
        {
            SetState(result.Stage);
        }

        // RUNNING
        public static State Running = State.CreateDefault(c => c
            .Caption("{Running}").Icon("Icons/Workflows/Production")
            .Action(w =>
            {
                if(w.Target.Start==null) w.Target.Start = DateTime.Now;
            })
        );

        public static Action Sign = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromState(()=>Running)
            .ToState(()=>Signed)
            .Action(w => w.Target.End = DateTime.Now)
        );

        // SIGNED
        public static State Signed = State.Create(c => c
            .Caption("{Signed}").Icon("Icons/Validations/Sign")
            .When(w=>w.Target.MandatoryDone)
        );

        public static Action Check = Action.Create(c => c
            .Caption("{Check}").Icon("Icons/Result/CheckPassed")
            .FromState(()=>Signed)
            .ToState(()=>Checked)
        );

        public static Action AskForCorrection = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/Workflows/Correct")
            .FromState(() => Signed, ()=>Checked)
            .ToState(() => CorrectionNeeded).Backward()
        );

        // ERROR
        public static State CorrectionNeeded = State.Create(c => c
            .Caption("{Correction Needed}").Icon("Icons/Workflows/Correct")
        );

        public static Action Correct = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromState(()=>CorrectionNeeded,()=>Signed)
            .ToState(()=>Running)
        );

        // CHECKED
        public static State Checked = State.Create(c => c
            .Caption("{Checked}").Icon("Icons/Results/CheckPassed")
        );

        public static Action Validate = Action.Create(c => c
            .Caption("{Validate}").Icon("Icons/Validations/Validated")
            .FromState(()=>Checked)
            .ToState(()=>Validated)
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );

        public static Action Invalidate = Action.Create(c => c
            .Caption("{Invalidate}").Icon("Icons/Validations/Invalidated")
            .FromState(()=>Checked,()=>Validated)
            .ToState(()=>Invalidated)
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );

        // VALIDATED
        public static State Validated = State.Create(c => c
            .Caption("{Validated}").Icon("Icons/Validations/Validated")
        );

        public static Action AskForCorrection3 = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/Workflows/Correct")
            .FromState(()=>Validated,()=>Invalidated)
            .When(w => w.Target.SampleTest.Stage == SampleTestWorkflow.Running.Name)
            .ToState(()=>CorrectionNeeded).Backward()
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );

        // INVALIDATED
        public static State Invalidated = State.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Validated")
        );


        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }
    }
}