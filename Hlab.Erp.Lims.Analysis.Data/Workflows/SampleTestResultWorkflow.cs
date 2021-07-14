using System;
using HLab.Erp.Acl;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Data.Workflows
{
    public class SampleTestResultWorkflow : Workflow<SampleTestResultWorkflow, SampleTestResult>
    {
        public SampleTestResultWorkflow(SampleTestResult result, IDataLocker locker) : base(result, locker)
        {
            SetStage(result.Stage);
        }

        // RUNNING
        public static Stage Running = Stage.CreateDefault(c => c
            .Caption("{Running}").Icon("Icons/Workflows/Production")
            .Action(w => { w.Target.Start ??= DateTime.Now; })
        );

        public static Action Sign = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromStage(() => Running)
            .When(w => w.Target.SampleTest.Stage == SampleTestWorkflow.Running)
            .WithMessage(w=>"{Test not in production}")
            .When(w => w.Target.SampleTest.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w=>"{Sample not in production}")
            .Action(w => w.Target.End = DateTime.Now)
            .ToStage(() => Signed)
        );

        // SIGNED
        public static Stage Signed = Stage.Create(c => c
            .Caption("{Signed}").Icon("Icons/Validations/Sign")
            .When(w => w.Target.MandatoryDone)
        );

        public static Action Check = Action.Create(c => c
            .Caption("{Check}").Icon("Icons/Result/CheckPassed")
            .FromStage(() => Signed)
            .Action(w => w.Target.End ??= DateTime.Now)
            .NeedRight(() => AnalysisRights.AnalysisResultCheck)
            .ToStage(() => Checked)
        );

        public static Action AskForCorrection = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/Workflows/Correct")
            .FromStage(() => Signed, () => Checked)
            .ToStage(() => CorrectionNeeded).Motivate().Backward()
        );

        // ERROR
        public static Stage CorrectionNeeded = Stage.Create(c => c
            .Caption("{Correction Needed}").Icon("Icons/Workflows/Correct")
        );

        public static Action Correct = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromStage(() => CorrectionNeeded, () => Signed)
            .ToStage(() => Running)
        );

        // CHECKED
        public static Stage Checked = Stage.Create(c => c
            .Caption("{Checked}").Icon("Icons/Results/CheckPassed")
        );

        public static Action Validate = Action.Create(c => c
            .Caption("{Validate}").Icon("Icons/Validations/Validated")
            .FromStage(() => Checked)
            .ToStage(() => Validated)
            .NeedRight(() => AnalysisRights.AnalysisResultValidate)
        );

        public static Action Invalidate = Action.Create(c => c
            .Caption("{Invalidate}").Icon("Icons/Validations/Invalidated")
            .FromStage(() => Checked, () => Validated)
            .ToStage(() => Invalidated)
            .NeedRight(() => AnalysisRights.AnalysisResultValidate).Motivate()
        );

        // VALIDATED
        public static Stage Validated = Stage.Create(c => c
            .Caption("{Validated}").Icon("Icons/Validations/Validated")
        );

        public static Action AskForCorrection3 = Action.Create(c => c
            .Caption("{Ask for correction}").Icon("Icons/Workflows/Correct")
            .FromStage(() => Validated, () => Invalidated)
            .When(w => w.Target.SampleTest?.Stage == SampleTestWorkflow.Running)
            .ToStage(() => CorrectionNeeded).Backward().Motivate()
            .NeedRight(() => AnalysisRights.AnalysisResultValidate)
        );

        // INVALIDATED
        public static Stage Invalidated = Stage.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Invalidated")
            .Action(w =>
            {
                if (w.Target.SampleTest.ResultId == w.Target.Id)
                    w.Target.SampleTest.Result = null;
            })
        );


        protected override Stage TargetStage
        {
            get => Target.Stage;
            set => Target.Stage = value;
        }
    }
}