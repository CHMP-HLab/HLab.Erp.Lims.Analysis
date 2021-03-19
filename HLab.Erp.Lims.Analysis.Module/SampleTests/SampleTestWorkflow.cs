using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{
    public class SampleTestWorkflow : Workflow<SampleTestWorkflow, SampleTest>
    {
        public SampleTestWorkflow(SampleTest test, IDataLocker locker):base(test,locker)
        {
            int id = test.Id;
            TestResults.AddFilter(() => e => e.SampleTestId == id);
                
            var task =  UpdateChildsAsync();
            SetStage(test.Stage);
            
            H<SampleTestWorkflow>.Initialize(this);
        }

        public async Task UpdateChildsAsync()
        {
            TestResults.Update(); // TODO : should be async
            Update();
        }

        [Import] private ObservableQuery<SampleTestResult> TestResults;
        private IProperty<bool> _ = H<SampleTestWorkflow>.Property<bool>(c => c

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
            .Caption("{Specifications}").Icon("Icons/Workflows/Specifications")
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
            .When(e => e.Target.SpecificationsDone)
            .WithMessage(w=>"{Missing} : {Specification}")
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
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
            .Caption("{Correction needed}").Icon("Icons/Workflows/Correct")
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
            .Caption("{Scheduling}").Icon("Icons/Workflows/Planning")
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
        );

        public static Action ToProduction  = Action.Create(c => c
            .Caption("{To Production}").Icon("Icons/Workflows/Production")
            .FromState(()=>Scheduling,()=>Scheduled)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production.Name)
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
            .Caption("{Running}").Icon("Icons/Workflows/Production")
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
                    foreach (var result in w.TestResults)
                    {
                        if (result.Stage == SampleTestResultWorkflow.Validated.Name)
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
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production.Name)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
            .Sign()
        );
        public static Action InvalidateResults = Action.Create(c => c
            .Caption("{Invalidate results}").Icon("Icons/Validations/Invalidated")
            .FromState(()=>Running,()=>ValidatedResults)
            .ToState(()=>InvalidatedResults)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production.Name)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
            .Sign().Motivate()
        );


        //########################################################
        // ValidatedResults

        public static Stage ValidatedResults = Stage.Create(c => c
            .Caption("{Validated}").Icon("Icons/Validations/Validated")
            .When(w =>
            {
                var validated = 0;
                var invalidated = 0;
                foreach(var result in w.TestResults)
                {
                    if(result.Stage==SampleTestResultWorkflow.Validated.Name) validated++;
                    else if(result.Stage==SampleTestResultWorkflow.Invalidated.Name) invalidated++;
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
                foreach(var result in w.TestResults)
                {
                    if(result.Stage==SampleTestResultWorkflow.Validated.Name) validated++;
                    else if(result.Stage==SampleTestResultWorkflow.Invalidated.Name) invalidated++;
                    else return false;
                }
                return (validated==1);
            })
            .WithMessage(w=>"No selected result")
        );

        public static Stage InvalidatedResults = Stage.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Invalidated")
        );

        protected override string StageName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }

    }
}