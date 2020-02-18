using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleTestResultWorkflow : Workflow<SampleTestResultWorkflow, SampleTestResult>
    {
        public SampleTestResultWorkflow(SampleTestResult result):base(result)
        {
            CurrentState = Running;
        }

        public static State Running = State.Create(c => c
            .Caption("{Running}").Icon("Icons/SampleTestResult/Running")
            .SetState(() => Running)
        );

        public static Action Sign = Action.Create(c => c
            .Caption("{Sign specifications}").Icon("Icons/SampleTest/Sign")
            .FromState(()=>Running)
            .ToState(()=>Signed)
        );

        public static State Signed = State.Create(c => c
            .Caption("{Signed}").Icon("Icons/SampleTestResult/Signed")
            .SetState(() => Signed)
        );

        public static Action Check = Action.Create(c => c
            .Caption("{Check}").Icon("Icons/SampleTest/Sign")
            .FromState(()=>Running)
            .ToState(()=>Checked)
        );

        public static State Checked = State.Create(c => c
            .Caption("{Signed}").Icon("Icons/SampleTestResult/Signed")
            .SetState(() => Checked)
        );

        public static Action Validate = Action.Create(c => c
            .Caption("{Sign specifications}").Icon("Icons/SampleTest/Sign")
            .FromState(()=>Running)
            .ToState(()=>Signed)
        );

        public static State Validated = State.Create(c => c
            .Caption("{Signed}").Icon("Icons/SampleTestResult/Signed")
            .SetState(() => Signed)
        );
    }
}