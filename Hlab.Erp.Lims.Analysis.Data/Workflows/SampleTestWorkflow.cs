using System;
using System.Linq;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Workflows
{
    public class SampleTestWorkflow : Workflow<SampleTestWorkflow, SampleTest>
    {
        private readonly ObservableQuery<SampleTestResult> _testResults;
        public SampleTestWorkflow(SampleTest test, IDataLocker locker, ObservableQuery<SampleTestResult> testResults) : base(test, locker)
        {
            _testResults = testResults;
            int id = test.Id;
            _testResults?.AddFilter(() => e => e.SampleTestId == id);
            _testResults?.Start();
            UpdateChildren();
            SetStage(test.Stage);

            H<SampleTestWorkflow>.Initialize(this);
        }

        public void UpdateChildren()
        {
            _testResults?.Update();
            Update();
        }

        private ITrigger _ = H<SampleTestWorkflow>.Trigger(c => c

            .On(e => e.Target.Stage)
            .Do((a, b) => a.SetStage(a.Target.Stage))

            .On(e => e.Locker.IsActive)
            .Do((a, b) => a.UpdateChildren())
        );

        //########################################################
        // Specifications

        public static Stage Specifications = Stage.CreateDefault(c => c
            .Caption("{Specifications}")
            .Icon("Icons/Workflows/Specifications")
            .Progress(0.0).Action(w => w.Target.Progress = 0.0)
        );

        public static Action SignSpecifications = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Workflows/SpecificationsSigned")
            .FromStage(() => Specifications)
            .NeedRight(() => AnalysisRights.AnalysisMonographSign)
            .ToStage(() => SignedSpecifications)
            .Sign()
        );

        //########################################################
        // Specifications
        //########################################################
        public static Stage SignedSpecifications = Stage.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/Specifications").SubIcon("Icons/Workflows/Sign")
            .When(e => e.Target.SpecificationDone)
            .WithMessage(w => "{Missing} : {Specification}")
            .NeedRight(() => AnalysisRights.AnalysisMonographSign)
            .Progress(0.1).Action(w => w.Target.Progress = 0.1)
        );

        public static Action RequestSpecificationsCorrection = Action.Create(c => c
            .Caption("{Request correction}").Icon("Icons/Workflows/Correct")
            .FromStage(() => SignedSpecifications, () => Scheduled, () => Scheduling, () => Running)
            .NeedRight(() => AnalysisRights.AnalysisMonographValidate)
            .ToStage(() => CorrectionNeeded)
            .Backward()
            .Sign().Motivate()
        );

        public static Action ValidateSpecifications = Action.Create(c => c
            .Caption("{Validate}").Icon("Icons/Validations/Validated")
            .FromStage(() => SignedSpecifications)
            .NeedRight(() => AnalysisRights.AnalysisMonographValidate)
            .ToStage(() => Scheduling)
            .Sign()
        );

        //########################################################
        //Correction Needed
        //########################################################

        public static Stage CorrectionNeeded = Stage.Create(c => c
            .Caption("{Correction needed}")
            .Icon("Icons/Workflows/Correct")
            .Progress(0.2).Action(w => w.Target.Progress = 0.2)
        );

        public static Action Correction = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromStage(() => CorrectionNeeded, () => SignedSpecifications, () => Scheduling, () => Scheduled, () => Running)
            .NeedRight(() => AnalysisRights.AnalysisMonographValidate)
            .ToStage(() => Specifications)
            .Backward().Motivate()
        );


        //########################################################
        // Scheduling

        public static Stage Scheduling = Stage.Create(c => c
            .Caption("{Scheduling}")
            .Icon("Icons/Workflows/Planning")
            .Progress(0.3).Action(w => w.Target.Progress = 0.3)
        );

        public static Action Schedule = Action.Create(c => c
           .Caption("{Schedule}").Icon("Icons/Workflows/Planning")
           .FromStage(() => Scheduling)
           .ToStage(() => Scheduled)
        );

        //########################################################
        // Scheduled

        public static Stage Scheduled = Stage.Create(c => c
            .Caption("{Scheduled}").Icon("Icons/Workflows/Planning")
            .When(w => w.Target.ScheduledDate != null)
            .WithMessage(w => "{Missing} : {Schedule Date}")
            .Progress(0.4).Action(w => w.Target.Progress = 0.4)
        );

        public static Action ToProduction = Action.Create(c => c
           .Caption("{To Production}").Icon("Icons/Workflows/Production")
           .FromStage(() => Scheduling, () => Scheduled)
           .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
           .WithMessage(w => "{Sample not in production}")
            .Action(w => w.Target.StartDate ??= DateTime.Now)
           .ToStage(() => Running)
        );

        public static Action Unschedule = Action.Create(c => c
           .Caption("{Unschedule}").Icon("Icons/Workflows/Planning")
           .FromStage(() => Scheduled)
           .ToStage(() => Scheduling)
           .Backward()
           .Motivate()
        );

        //########################################################
        // Running

        public static Stage Running = Stage.Create(c => c
            .Caption("{Running}")
            .Icon("Icons/Workflows/Production")
            .Progress(w => 0.5 + (w._testResults.Sum(r => r.Progress) / w._testResults.Count) * 0.4)
            .Action(w => w.Target.Progress = 0.5)
        );

        public static Action Stop = Action.Create(c => c
           .Caption("{Pause}").Icon("Icons/Workflows/Pause")
           .FromStage(() => Running)
           .ToStage(() => Scheduling)
           .Backward()
           .Motivate()
        );

        public static Action ValidateResults = Action.Create(c => c
            .Caption("{Validate results}").Icon("Icons/Validations/Validated")
            .FromStage(() => Running)
            .Action(w =>
            {
                // if StartDate or EndDate is empty set it
                if (!w.Target.StartDate.HasValue) w.Target.StartDate = DateTime.Now;
                if (!w.Target.EndDate.HasValue) w.Target.EndDate = DateTime.Now;

                // if no result selected, select the first (so unique as required by ValidatedResults stage)
                if (w.Target.Result == null)
                {
                    foreach (var result in w._testResults)
                    {
                        if (result.Stage == SampleTestResultWorkflow.Validated)
                        {
                            w.Target.Result = result;
                            break;
                        }
                    }
                }
            })
            .ToStage(() => ValidatedResults)
            .WhenStageAllowed(() => ValidatedResults)
            //Todo : reuse toState
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w => "{Sample not in production}")
            .NeedRight(() => AnalysisRights.AnalysisResultValidate)
            .Sign()
        );

        public static Action InvalidateResults = Action.Create(c => c
            .Caption("{Invalidate results}").Icon("Icons/Validations/Invalidated")
            .FromStage(() => Running, () => ValidatedResults)
            .ToStage(() => InvalidatedResults)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w => "{Sample not in production}")
            .NeedRight(() => AnalysisRights.AnalysisResultValidate)
            .Sign().Motivate()
        );


        //########################################################
        // ValidatedResults

        public static Stage ValidatedResults = Stage.Create(c => c
            .Caption("{Validated}").Icon("Icons/Validations/Validated")
            .Progress(1.0).Action(w => w.Target.Progress = 1.0)
            .When(w =>
            {
                foreach (var result in w._testResults)
                {
                    if (result.Stage != SampleTestResultWorkflow.Validated && result.Stage != SampleTestResultWorkflow.Invalidated)
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage(w => "{Some results not validated yet}")

            .When(w =>
            {
                foreach (var result in w._testResults)
                {
                    if (result.Stage == SampleTestResultWorkflow.Validated) return true;
                }
                return false;
            })
            .WithMessage(w => "{No validated test}")

            .When(w =>
            {
                if (w.Target.Result != null) return true;

                var validated = 0;
                foreach (var result in w._testResults)
                {
                    if (result.Stage == SampleTestResultWorkflow.Validated) validated++;
                    if (validated > 1) return false;
                }
                return validated == 1;
            })
            .WithMessage(w => "{No selected result}")

            .When(w => !w.Target.StartDate.HasValue || w.Target.StartDate <= DateTime.Now)
            .WithMessage(w => "{Start date should be past}")

            .When(w => !w.Target.EndDate.HasValue || w.Target.EndDate <= DateTime.Now)
            .WithMessage(w => "{End date should be past}")

            .When(w => !w.Target.EndDate.HasValue || !w.Target.StartDate.HasValue || w.Target.EndDate >= w.Target.StartDate)
            .WithMessage(w => "{End date should be after start}")
        );

        public static Stage InvalidatedResults = Stage.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Invalidated")
        );

        public static Action Correct = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromStage(() => ValidatedResults, () => InvalidatedResults)
            .NeedRight(() => AnalysisRights.AnalysisResultValidate)
            .ToStage(() => Running)
        );

        protected override Stage TargetStage
        {
            get => Target.Stage;
            set => Target.Stage = value;
        }

    }
}