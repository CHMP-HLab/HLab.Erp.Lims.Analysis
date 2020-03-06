using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleTestWorkflow : Workflow<SampleTestWorkflow, SampleTest>
    {
        public SampleTestWorkflow(SampleTest test, IDataLocker locker):base(test,locker)
        {
            SetState(test.Stage);
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

        public static State Specifications = State.CreateDefault(c => c
            .Caption("{Specifications}").Icon("Icons/Workflows/Specifications")
//            .SetState(() => Specifications)
        );

        public static Action SignSpecifications = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Workflows/SpecificationsSigned")
            .FromState(()=>Specifications)
            .ToState(()=>SignedSpecifications)
        );

        public static State SignedSpecifications = State.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/Specifications").SubIcon("Icons/Workflows/Sign")
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
            .SetState(() => SignedSpecifications)
        );

        //Request correction
        public static Action RequestSpecificationsCorrection = Action.Create(c => c
            .Caption("{Request correction}").Icon("Icons/Workflows/Correction")
            .FromState(()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> CorrectionNeeded)
            .Backward()
        );

        public static State CorrectionNeeded = State.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/Specifications").SubIcon("Icons/Workflows/Sign")
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
            .SetState(() => CorrectionNeeded)
        );

        public static Action Correction = Action.Create(c => c
            .Caption("{Request correction}").Icon("Icons/Workflows/Correction")
            .FromState(()=>CorrectionNeeded)
            //.NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> Specifications)
            .Backward()
        );


        public static Action ValidateSpecifications = Action.Create(c => c
            .Caption("{Schedule}").Icon("Icons/SampleTest/Validate")
            .FromState(()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
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
            .FromState(()=>Running)
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

        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }
    }
}