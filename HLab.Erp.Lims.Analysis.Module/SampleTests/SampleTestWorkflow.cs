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
            SetState(test.Stage);
        }

        public async Task UpdateChildsAsync()
        {
            await TestResults.UpdateAsync();
            Update();
        }

        [Import] private ObservableQuery<SampleTestResult> TestResults;
        private IProperty<bool> _ = H.Property<bool>(c => c

            .On(e => e.Target.Stage)
            .Do((a, b) =>
            {
                a.SetState(a.Target.Stage);
            })

            .On(e => e.Locker.IsActive)
            .Do(async (a, b) =>
            {
                await a.UpdateChildsAsync();
            })
        );

        //########################################################
        // Specifications

        public static State Specifications = State.CreateDefault(c => c
            .Caption("{Specifications}").Icon("Icons/Workflows/Specifications")
        );

        public static Action SignSpecifications = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Workflows/SpecificationsSigned")
            .FromState(()=>Specifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
            .ToState(()=>SignedSpecifications)
        );

        //########################################################
        // Specifications
        //########################################################
        public static State SignedSpecifications = State.Create(c => c
            .Caption("{Specifications Signed}").Icon("Icons/Workflows/Specifications").SubIcon("Icons/Workflows/Sign")
            .When(e => e.Target.SpecificationsDone)
            .WithMessage(w=>"{Missing} : {Specification}")
            .NeedRight(()=>AnalysisRights.AnalysisMonographSign)
        );

        public static Action RequestSpecificationsCorrection = Action.Create(c => c
            .Caption("{Request correction}").Icon("Icons/Workflows/Correct")
            .FromState(()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> CorrectionNeeded)
            .Backward()
        );

        public static Action ValidateSpecifications = Action.Create(c => c
            .Caption("{Validate}").Icon("Icons/Validations/Validated")
            .FromState(()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=>Scheduling)
        );

        //########################################################
        //Correction Needed
        //########################################################

        public static State CorrectionNeeded = State.Create(c => c
            .Caption("{Correction needed}").Icon("Icons/Workflows/Correct")
        );

        public static Action Correction = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromState(()=>CorrectionNeeded,()=>SignedSpecifications)
            .NeedRight(()=>AnalysisRights.AnalysisMonographValidate)
            .ToState(()=> Specifications)
            .Backward()
        );


        //########################################################
        // Scheduling

        public static State Scheduling = State.Create(c => c
            .Caption("{Scheduling}").Icon("Icons/Workflows/Planning")
        );

        public static Action Schedule  = Action.Create(c => c
            .Caption("{Schedule}").Icon("Icons/Workflows/Planning")
            .FromState(()=>Scheduling)
            .ToState(()=>Scheduled)
        );

        //########################################################
        // Scheduled

        public static State Scheduled = State.Create(c => c
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
        );

        //########################################################
        // Running

        public static State Running = State.Create(c => c
            .Caption("{Running}").Icon("Icons/Workflows/Production")
        );

        public static Action Stop  = Action.Create(c => c
            .Caption("{Pause}").Icon("Icons/Workflows/Pause")
            .FromState(()=>Running)
            .ToState(()=>Scheduling)
            .Backward()
        );

        public static Action ValidateResults = Action.Create(c => c
            .Caption("{Validate results}").Icon("Icons/Validations/Validated")
            .FromState(()=>Running)
            .ToState(()=>ValidatedResults)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production.Name)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );
        public static Action InvalidateResults = Action.Create(c => c
            .Caption("{Invalidate results}").Icon("Icons/Validations/Invalidated")
            .FromState(()=>Running,()=>ValidatedResults)
            .ToState(()=>InvalidatedResults)
            .When(w => w.Target.Sample.Stage == SampleWorkflow.Production.Name)
            .WithMessage(w=>"{Sample not in production}")
            .NeedRight(()=>AnalysisRights.AnalysisResultValidate)
        );


        //########################################################
        // ValidatedResults

        public static State ValidatedResults = State.Create(c => c
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
            .NotWhen(w => w.Target.Result == null)
            .WithMessage(w=>"No selected result")
        );

        public static State InvalidatedResults = State.Create(c => c
            .Caption("{Invalidated}").Icon("Icons/Validations/Invalidated")
        );

        protected override string StateName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }
    }
}