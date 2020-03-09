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
            CurrentState = Running;
        }

        // RUNNING
        public static State Running = State.CreateDefault(c => c
            .Caption("{Running}").Icon("Icons/SampleTestResult/Running")
            .SetState(() => Running)
        );

        public static Action Sign = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/SampleTest/Sign")
            .FromState(()=>Running)
            .ToState(()=>Signed)
        );

        // SIGNED
        public static State Signed = State.Create(c => c
            .Caption("{Signed}").Icon("Icons/SampleTestResult/Signed")
            .SetState(() => Signed)
        );

        public static Action Check = Action.Create(c => c
            .Caption("{Check}").Icon("Icons/SampleTest/Sign")
            .FromState(()=>Signed)
            .ToState(()=>Checked)
        );

        public static Action AskForCorrection = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/SampleTest/Correction")
            .FromState(()=>Signed)
            .ToState(()=>CorrectionNeeded).Backward()
        );

        // ERROR
        public static State CorrectionNeeded = State.Create(c => c
            .Caption("{Correction Needed}").Icon("Icons/Workflows/Correct")
            .SetState(() => CorrectionNeeded)
        );

        public static Action Correct = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromState(()=>CorrectionNeeded,()=>Signed)
            .ToState(()=>Running)
        );

        // CHECKED
        public static State Checked = State.Create(c => c
            .Caption("{Checked}").Icon("Icons/SampleTestResult/Signed")
            .SetState(() => Checked)
        );

        public static Action AskForCorrection2 = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/SampleTest/Correction")
            .FromState(()=>Checked)
            .ToState(()=>CorrectionNeeded).Backward()
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
            .Caption("{Validated}").Icon("Icons/SampleTestResult/Validated")
            .SetState(() => Validated)
        );

        public static Action AskForCorrection3 = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/SampleTest/Correction")
            .FromState(()=>Validated,()=>Invalidated)
            .When(w => w.Target.SampleTest.Stage == SampleTestWorkflow.Running.Name)
            .ToState(()=>CorrectionNeeded).Backward()
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );

        // INVALIDATED
        public static State Invalidated = State.Create(c => c
            .Caption("{Validated}").Icon("Icons/SampleTestResult/Validated")
            .SetState(() => Validated)
        );


        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }
    }
}