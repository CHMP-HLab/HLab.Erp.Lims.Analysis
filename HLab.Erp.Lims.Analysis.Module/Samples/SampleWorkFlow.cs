using System;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Samples
{


    public class SampleWorkflow : Workflow<SampleWorkflow,Sample>
    {
        public SampleWorkflow(Sample sample, DataLocker<Sample> locker):base(sample,locker)
        {
            var id = sample.Id;
            SampleTests.AddFilter(() => e => e.SampleId == id);

            H<SampleWorkflow>.Initialize(this);
                
            var task = UpdateChildrenAsync();
            SetStage(sample.Stage);
        }
        public async Task UpdateChildrenAsync()
        {
            await SampleTests.UpdateAsync();
            Update();
        }

        [Import] private ObservableQuery<SampleTest> SampleTests;

        protected override string StageName
        {
            get => Target.Stage; 
            set => Target.Stage = value;
        }

        private IProperty<bool> _ = H<SampleWorkflow>.Property<bool>(c => c

            .On(e => e.Target.Stage)
            .Do((a,p) =>
            {
                a.SetStage(a.Target.Stage);
            })

            .On(e => e.Locker.IsActive)
            .Do(async (swf,p) =>
            {
                await swf.UpdateChildrenAsync();
            })
        );


        //########################################################
        // RECEPTION

        public static Stage Reception = Stage.CreateDefault(c => c
            .Caption("{Reception entry}").Icon("Icons/Sample/PackageOpened|Icons/Validations/Edit")
            .SetState(() => Reception)
        );

        public static Action SignReception = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromState(() => Reception)
            .ToState(() => ReceptionCheck)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionSign)
            .Sign()
        );

        //########################################################
        // RECEPTION CHECK

        public static Stage ReceptionCheck = Stage.Create(c => c
            .Caption("{Reception check}").Icon("Icons/Sample/PackageOpened|Icons/Validations/Sign")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Batch))
            .WithMessage(w => "{Missing} : {Batch No}")
            .HighlightField(w => w.Target.Batch)

            .NotWhen(w => w.Target.ReceivedQuantity == null)
            .WithMessage(w => "{Missing} : {Received quantity}")
            .HighlightField(w => w.Target.ReceivedQuantity)

            .NotWhen(w => w.Target.ReceptionDate == null)
            .WithMessage(w => "{Missing} : {Reception date}")
            .HighlightField(w => w.Target.ReceptionDate)

            .NotWhen(w => w.Target.ProductId == null)
            .WithMessage(w => "{Missing} : {Product}")
            .HighlightField(w => w.Target.Product)

            .NotWhen(w => w.Target.CustomerId == null)
            .WithMessage(w => "{Missing} : {Customer}")
            .HighlightField(w => w.Target.Customer)

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Reference))
            .WithMessage(w => "{Missing} : {Reference}")
            .HighlightField(w => w.Target.Reference)

            .NotWhen(w => 
                w.Target.ExpirationDate!=null 
                && w.Target.ManufacturingDate!=null 
                && w.Target.ExpirationDate < w.Target.ManufacturingDate)
            .WithMessage(w => "{Expiry and Manufacturing dates mismatch}")
            .HighlightField(w => w.Target.ExpirationDate).HighlightField(w=>w.Target.ManufacturingDate)
        );


        public static Action CheckReception = Action.Create(c => c
            .Caption(w => "{Check}").Icon(w => "Icons/Workflows/Check")
            .FromState(() => ReceptionCheck)
            .ToState(() => Monograph)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
            .Sign()
        );

        public static Action ReceptionAskForCorrection = Action.Create(c => c
            .Caption(w => "{Ask for correction}").Icon(w => "Icons/Workflows/Correct")
            .FromState(() => ReceptionCheck)
            .ToState(() => ReceptionCorrectionAsked)
            .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
            .Backward()
            .Sign()
            .Motivate()
        );

        //########################################################
        // RECEPTION CORRECTION

        public static Stage ReceptionCorrectionAsked = Stage.Create(c => c
            .Caption(w => "{Reception correction asked}")
            .Icon(w => "Icons/Workflows/Correct")
        );

        public static Action CorrectReception = Action.Create(c => c
           .Caption(w => "{Correct reception}").Icon(w => "Icons/Workflows/Correct")
           .FromState(() => ReceptionCorrectionAsked,()=>ReceptionCheck,()=>Monograph)
           .ToState(() => Reception)
           .NeedRight(()=>AnalysisRights.AnalysisReceptionSign)
           .Backward()
           .Sign()
           .Motivate()
        );

        //########################################################
        // MONOGRAPH

        public static Stage Monograph = Stage.Create(c => c
            .Caption(w => "{Monograph Entry}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Edit")
            .WhenStateAllowed(() => ReceptionCheck)
        );

        public static Action ValidateMonograph = Action.Create(c => c
           .Caption(w => "{Validate monograph}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")
           .NeedRight(()=>AnalysisRights.AnalysisReceptionCheck)
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
            .Sign()
            );

        //########################################################
        // MONOGRAPH VALIDATED

        public static Stage MonographClosed = Stage.Create(c => c
            .Caption(w => "{Monograph validated}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "{Missing} : {Pharmacopoeia}")
                .HighlightField(w => w.Target.Pharmacopoeia)

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "{Missing} : {Pharmacopoeia version}")
                .HighlightField(w => w.Target.PharmacopoeiaVersion)

            .NotWhen(w => w.SampleTests.Count == 0)
                .WithMessage(w => "{Missing} : {Tests}")
//                .HighlightField(w => w.Target.Pharmacopoeia)

            .When(w => {
                foreach (SampleTest test in w.SampleTests)
                {
                    if (test.Stage == SampleTestWorkflow.Specifications.Name) return false; 
                    if (test.Stage == SampleTestWorkflow.SignedSpecifications.Name) return false;
                }
                return true;
            })
                .WithMessage(w => "{Missing} : {Test specifications}")
        );

        public static Action ValidatePlanning = Action.Create(c => c
           .Caption(w => "{Schedule}").Icon(w => "Icons/Workflows/Planning|Icons/Validations/Validated")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );

        //########################################################
        // PLANNING

        public static Stage Planning = Stage.Create(c => c
            .Caption(w => "{Scheduling}").Icon(w => "icons/Workflows/Planning|Icons/Validations/Edit")
            .WhenStateAllowed(() => MonographClosed)
        );

        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "{Put into production}").Icon(w => "Icons/Workflows/Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedRight(()=>AnalysisRights.AnalysisSchedule)
        );


        //########################################################
        // PRODUCTION
        public static Stage Production = Stage.Create(c => c
            .Caption(w => "{Production}").Icon(w => "Icons/Workflows/Production")
            .WhenStateAllowed(() => MonographClosed)
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create( c => c
            .Caption(w => "{Validate Certificate}").Icon(w => "Icons/Workflows/Certificate")
            .FromState(() => Production)
            .Action(w=>{
                w.Target.NotificationDate = DateTime.Today;
                w.Target.Validator = WorkflowAnalysisExtension.Acl.Connection.User;
                })
            .ToState(() => Certificate)
            .NeedRight(()=>AnalysisRights.AnalysisCertificateCreate)
            .Sign()
        );

        public static Action AbortSample = Action.Create( c => c
            .Caption(w => "{Abort analysis}").Icon(w => "Icons/Workflows/Aborted")
            .FromState(()=>Reception, ()=>Monograph, ()=>Planning, () => Production, ()=>Certificate, ()=>Closed)
            .Action(w=>{
                w.Target.Validator = WorkflowAnalysisExtension.Acl.Connection.User;
                })
            .ToState(() => Aborted)
            .NeedRight(()=>AnalysisRights.AnalysisCertificateCreate)
            .Sign().Motivate()
        );

        public static Stage Certificate = Stage.Create(c => c
            .Caption(w => "{Certificate}").Icon(w => "Icons/Workflows/Certificate")
            .WhenStateAllowed(()=>MonographClosed)
            .When(w => {
                var validated = 0;
                var invalidated = 0;
                foreach (SampleTest test in w.SampleTests) 
                {
                    if (test.Stage == SampleTestWorkflow.ValidatedResults.Name) validated++; 
                    else if (test.Stage == SampleTestWorkflow.InvalidatedResults.Name) invalidated++; 
                    else return false;
                }

                return validated>0;
            })
            .WithMessage(w=>"{Some tests not validated yet}")
        );

        public static Stage Aborted = Stage.Create(c => c
            .Caption(w => "{Aborted}").Icon(w => "Icons/Workflows/Aborted")
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create( c => c
            .Caption(w => "{Close}").Icon(w => "Icons/Workflows/Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
            .Sign()
        );

        public static Stage Closed = Stage.Create(c => c
            .Caption(w => "{Closed}").Icon(w => "Icons/Workflows/Closed")
            .SetState(() => Closed)
            //.When(w => w.Target.Invoiced).WithMessage(w => "{Not billed}")
            //.When(w => w.Target.Paid).WithMessage(w => "{Not Payed}")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
