using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleTestWorkflow : Workflow<SampleTestWorkflow, SampleTest>
    {
        public SampleTestWorkflow(SampleTest test):base(test)
        {
            SetState(test.Stage);
        }


        protected override bool OnSetState(State state)
        {
            Target.Stage = state.Name;
            return true;
        }

        private IProperty<bool> _ = H.Property<bool>(c => c
            .On(e => e.Target.Stage)
            .Do((a, b) =>
            {
                a.SetState(a.Target.Stage);
            })
        );

        //########################################################
        // Specifications

        public static State Specifications = State.Create(c => c
            .Caption("{Specifications}").Icon("Icons/Workflows/Specifications")
            .SetState(() => Specifications)
        );

        public static Action SignSpecifications = Action.Create(c => c
            .Caption("{Sign specifications}").Icon("Icons/Workflows/SpecificationsSigned")
            .FromState(()=>Specifications)
            .ToState(()=>SignedSpecifications)
        );

        public static State SignedSpecifications = State.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/SpecificationsSigned")
            .SetState(() => SignedSpecifications)
        );

        public static Action ValidateSpecifications = Action.Create(c => c
            .Caption("{Sign specifications}").Icon("Icons/SampleTest/Validate")
            .FromState(()=>SignedSpecifications)
            .ToState(()=>Scheduling)
        );

        //########################################################
        // Scheduling

        public static State Scheduling = State.Create(c => c
            .Caption("{Scheduling}").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Scheduling)
        );

        public static Action Production  = Action.Create(c => c
            .Caption("{Production}").Icon("Icons/SampleTest/Production")
            .FromState(()=>Scheduling)
            .ToState(()=>Running)
        );

        public static State Running = State.Create(c => c
            .Caption("{Running}").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Running)
        );

        public static Action ValidateResults = Action.Create(c => c
            .Caption("{Validate results}").Icon("Icons/SampleTest/Validate")
            .FromState(()=>SignedSpecifications)
            .ToState(()=>ValidatedResults)
        );

        public static Action AskForRetest = Action.Create(c => c
            .Caption("{Ask for retest}").Icon("Icons/SampleTest/Validate")
            .FromState(()=>SignedSpecifications)
            .ToState(()=>Scheduling)
        );

        public static State ValidatedResults = State.Create(c => c
            .Caption("{Validated}").Icon("Icons/Sample/PackageOpened")
            .SetState(() => ValidatedResults)
        );
    }
}