using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Workflows
{
    public class SampleTestWorkflow : Workflow<SampleTestWorkflow, SampleTest>
    {
        private readonly ObservableQuery<SampleTestResult> _testResults;
        public SampleTestWorkflow(SampleTest test, IDataLocker locker, ObservableQuery<SampleTestResult> testResults):base(test,locker)
        {
            _testResults = testResults;
            int id = test.Id;
            _testResults.AddFilter(() => e => e.SampleTestId == id);
                
            var task =  UpdateChildsAsync();
            SetStage(test.Stage);
            
            H<SampleTestWorkflow>.Initialize(this);
        }

        public async Task UpdateChildsAsync()
        {
            _testResults.Update();
            Update();
        }

        private ITrigger _ = H<SampleTestWorkflow>.Trigger(c => c

            .On(e => e.Target.Stage)
            .Do((a, b) =>
            {
                a.SetStage(a.Target.Stage);
            })

            .On(e => e.Locker.IsActive)
            .Do(async (a, b) =>
            {
                await a.UpdateChildsAsync();
            })
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
            .FromState(()=>Specifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
            .ToState(()=>SignedSpecifications)
            .Sign()
        );

        //########################################################
        // Specifications
        //########################################################
        public static Stage SignedSpecifications = Stage.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/Specifications").SubIcon("Icons/Workflows/Sign")
            .When(e => e.Target.SpecificationDone)
            .WithMessage(w=>"{Missing} : {Specification}")
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
            .Progress(0.1).Action(w => w.Target.Progress = 0.1)
        );

        public static Action RequestSpecificationsCorrection = Action.Create(c => c
            .Caption("{Request correction}").Icon("Icons/Workflows/Correct")
            .FromState(()=>SignedSpecifications, ()=>Scheduled, ()=>Scheduling, ()=>Running)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> CorrectionNeeded)
            .Backward()
            .Sign().Motivate()
        );

        public static Action ValidateSpecifications = Action.Create(c => c
            .Caption("{Validate}").Icon("Icons/Validations/Validated")
            .FromState(()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=>Scheduling)
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
            .FromState(()=>CorrectionNeeded,()=>SignedSpecifications, ()=>Scheduling, ()=>Scheduled,()=>Running)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> Specifications)
            .Backward().Motivate()
        );


        //########################################################
        // Scheduling

        public static Stage Scheduling = Stage.Create(c => c
            .Caption("{Scheduling}")
            .Icon("Icons/Workflows/Planning")
            .Progress(0.3).Action(w => w.Target.Progress = 0.3)
        );

        public static Action Schedule  = Action.Create(c => c
            .Caption("{Schedule}").Icon("Icons/Workflows/Planning")
            .FromState(()=>Scheduling)
            .ToState(()=>Scheduled)
        );

        //########################################################
        // Scheduled

        public static Stage Scheduled = Stage.Create(c => c
            .Caption("{Scheduled}").Icon("Icons/Workflows/Planning")
            .When(w=>w.Target.ScheduledDate!=null)
            .WithMessage(w=>"{Missing} : {Schedule Date}")
            .Progress(0.4).Action(w => w.Target.Progress = 0.4)
        );

        public static Action ToProduction  = Action.Create(c => c
            .Caption("{To Production}").Icon("Icons/Workflows/Production")
            .FromState(()=>Scheduling,()=>Scheduled)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w=>"{Sample not in production}")
            .ToState(()=>Running)
        );

        public static Action Unschedule  = Action.Create(c => c
            .Caption("{Unschedule}").Icon("Icons/Workflows/Planning")
            .FromState(()=>Scheduled)
            .ToState(()=>Scheduling)
            .Backward()
            .Motivate()
        );

        //########################################################
        // Running

        public static Stage Running = Stage.Create(c => c
            .Caption("{Running}")
            .Icon("Icons/Workflows/Production")
            .Progress(w => 0.5 + (w._testResults.Sum(r => r.Progress) / w._testResults.Count)*0.4)
            .Action(w => w.Target.Progress = 0.5)
        );

        public static Action Stop  = Action.Create(c => c
            .Caption("{Pause}").Icon("Icons/Workflows/Pause")
            .FromState(()=>Running)
            .ToState(()=>Scheduling)
            .Backward()
            .Motivate()
        );

        public static Action ValidateResults = Action.Create(c => c
            .Caption("{Validate results}").Icon("Icons/Validations/Validated")
            .FromState(()=>Running)
            .Action(w =>
            {
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
            .ToState(()=>ValidatedResults)
            .WhenStateAllowed(()=>ValidatedResults)    
        //Todo : reuse toState
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
            .Sign()
        );
        public static Action InvalidateResults = Action.Create(c => c
            .Caption("{Invalidate results}").Icon("Icons/Validations/Invalidated")
            .FromState(()=>Running,()=>ValidatedResults)
            .ToState(()=>InvalidatedResults)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
            .Sign().Motivate()
        );


        //########################################################
        // ValidatedResults

        public static Stage ValidatedResults = Stage.Create(c => c
            .Caption("{Validated}").Icon("Icons/Validations/Validated")
            .Progress(1.0).Action(w => w.Target.Progress = 1.0)
            .When(w =>
            {
                var validated = 0;
                var invalidated = 0;
                foreach(var result in w._testResults)
                {
                    if(result.Stage==SampleTestResultWorkflow.Validated) validated++;
                    else if(result.Stage==SampleTestResultWorkflow.Invalidated) invalidated++;
                    else return false;
                }
                return (validated>0);
            })
            .WithMessage(w=>"Some results not validated yet")
            .When(w =>
            {
                if(w.Target.Result != null) return true;

                var validated = 0;
                var invalidated = 0;
                foreach(var result in w._testResults)
                {
                    if(result.Stage==SampleTestResultWorkflow.Validated) validated++;
                    else if(result.Stage==SampleTestResultWorkflow.Invalidated) invalidated++;
                    else return false;
                }
                return (validated==1);
            })
            .WithMessage(w=>"No selected result")
            
        );

        public static Stage InvalidatedResults = Stage.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Invalidated")
        );

        protected override Stage TargetStage
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }

    }
}