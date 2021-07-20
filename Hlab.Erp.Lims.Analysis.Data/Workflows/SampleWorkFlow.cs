using System;
using HLab.Erp.Acl;
using HLab.Erp.Data.Observables;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Workflows
{


    public class SampleWorkflow : Workflow<SampleWorkflow, Sample>
    {
        private readonly ObservableQuery<SampleTest> _sampleTests;
        public SampleWorkflow(Sample sample, IDataLocker<Sample> locker, ObservableQuery<SampleTest> sampleTests) : base(sample, locker)
        {
            _sampleTests = sampleTests;
            var id = sample.Id;
            _sampleTests?.AddFilter(() => e => e.SampleId == id);

            H<SampleWorkflow>.Initialize(this);

            _sampleTests?.Start();
            UpdateChildren();

            SetStage(sample.Stage);
        }

        public void UpdateChildren()
        {
            _sampleTests?.Update(Update);
        }

        protected override Stage TargetStage
        {
            get => Target.Stage;
            set => Target.Stage = value;
        }

        protected override Stage TargetPreviousStage
        {
            get => StageFromName(Target.PreviousStageId);
            set => Target.PreviousStageId = value.Name;
        }

        private ITrigger _ = H<SampleWorkflow>.Trigger(c => c

            .On(e => e.Target.Stage)
            .Do((a, p) =>
            {
                a.SetStage(a.Target.Stage);
            })

            .On(e => e.Locker.IsActive)
            .Do((swf, p) =>
            {
                swf.UpdateChildren();
            })
        );


        //########################################################
        // RECEPTION

        public static Stage Reception = Stage.CreateDefault(c => c
            .Caption("{Reception entry}").Icon("Icons/Sample/PackageOpened|Icons/Validations/Edit")
            .SetStage(() => Reception)
        );

        public static Action SignReception = Action.Create(c => c
            .Caption("{Sign}").Icon("Icons/Validations/Sign")
            .FromStage(() => Reception)
            .ToStage(() => ReceptionCheck)
            .NeedRight(() => AnalysisRights.AnalysisReceptionSign)
            .Sign()
        );

        //########################################################
        // RECEPTION CHECK

        public static Stage ReceptionCheck = Stage.Create(c => c
            .Caption("{Reception check}")
            .Icon("Icons/Sample/PackageOpened|Icons/Validations/Sign")
            .Progress(0.1).Action(w => w.Target.Progress = 0.1)

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
                w.Target.ExpirationDate != null
                && w.Target.ManufacturingDate != null
                && w.Target.ExpirationDate < w.Target.ManufacturingDate)
            .WithMessage(w => "{Expiry and Manufacturing dates mismatch}")
            .HighlightField(w => w.Target.ExpirationDate).HighlightField(w => w.Target.ManufacturingDate)
        );


        public static Action CheckReception = Action.Create(c => c
            .Caption(w => "{Check}").Icon(w => "Icons/Workflows/CheckResult")
            .FromStage(() => ReceptionCheck)
            .ToStage(() => Monograph)
            .NeedRight(() => AnalysisRights.AnalysisReceptionCheck)
            .Sign()
        );

        public static Action ReceptionAskForCorrection = Action.Create(c => c
            .Caption(w => "{Ask for correction}").Icon(w => "Icons/Workflows/Correct")
            .FromStage(() => ReceptionCheck)
            .ToStage(() => ReceptionCorrectionAsked)
            .NeedRight(() => AnalysisRights.AnalysisReceptionCheck)
            .Backward()
            .Sign()
            .Motivate()
        );

        //########################################################
        // RECEPTION CORRECTION

        public static Stage ReceptionCorrectionAsked = Stage.Create(c => c
            .Caption(w => "{Reception correction asked}")
            .Icon(w => "Icons/Workflows/Correct")
            .Progress(0.2).Action(w => w.Target.Progress = 0.2)
        );

        public static Action CorrectReception = Action.Create(c => c
           .Caption(w => "{Correct reception}").Icon(w => "Icons/Workflows/Correct")
           .FromStage(() => ReceptionCorrectionAsked, () => ReceptionCheck, () => Monograph)
           .ToStage(() => Reception)
           .NeedRight(() => AnalysisRights.AnalysisReceptionSign)
           .Backward()
           .Sign()
           .Motivate()
        );

        //########################################################
        // MONOGRAPH

        public static Stage Monograph = Stage.Create(c => c
            .Caption(w => "{Monograph Entry}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Edit")
            .WhenStageAllowed(() => ReceptionCheck)
            .Progress(0.3).Action(w => w.Target.Progress = 0.3)
        );

        public static Action ValidateMonograph = Action.Create(c => c
           .Caption(w => "{Validate monograph}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")
           .NeedRight(() => AnalysisRights.AnalysisMonographValidate)
           .FromStage(() => Monograph)
           .ToStage(() => MonographClosed)
            .Sign()
            );

        //########################################################
        // MONOGRAPH REVIEW NEEDED

        public static Stage MonographReviewNeeded = Stage.Create(c => c
            .Caption(w => "{Monograph Review Needed}").Icon(w => "Icons/Workflows/Monograph|Icons/Workflows/Correct")
            .WhenStageAllowed(() => Monograph)
        );

        public static Action CorrectMonograph = Action.Create(c => c
            .Caption("{Correct}").Icon("Icons/Workflows/Correct")
            .FromStage(() => MonographReviewNeeded)
            .NeedRight(() => AnalysisRights.AnalysisMonographValidate)
            .ToStage(() => Monograph)
        );

        //########################################################
        // MONOGRAPH VALIDATED

        public static Stage MonographClosed = Stage.Create(c => c
            .Caption(w => "{Monograph validated}").Icon(w => "Icons/Workflows/Monograph|Icons/Validations/Validated")
            .Progress(0.4).Action(w => w.Target.Progress = 0.4)

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "{Missing} : {Pharmacopoeia}")
                .HighlightField(w => w.Target.Pharmacopoeia)

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "{Missing} : {Pharmacopoeia version}")
                .HighlightField(w => w.Target.PharmacopoeiaVersion)

            .NotWhen(w => (w._sampleTests?.Count ?? 0) == 0)
                .WithMessage(w => "{Missing} : {Tests}")
            //                .HighlightField(w => w.Target.Pharmacopoeia)

            .When(w =>
            {
                if (w._sampleTests != null)
                    foreach (SampleTest test in w._sampleTests)
                    {
                        if (test.Stage == SampleTestWorkflow.Specifications) return false;
                        if (test.Stage == SampleTestWorkflow.SignedSpecifications) return false;
                    }
                return true;
            })
                .WithMessage(w => "{Missing} : {Test specifications}")
        );

        public static Action ValidatePlanning = Action.Create(c => c
           .Caption(w => "{Schedule}").Icon(w => "Icons/Workflows/Planning|Icons/Validations/Validated")
           .FromStage(() => MonographClosed)
           .ToStage(() => Planning)
           .NeedRight(() => AnalysisRights.AnalysisSchedule)
        );

        //########################################################
        // PLANNING

        public static Stage Planning = Stage.Create(c => c
            .Caption(w => "{Scheduling}").Icon(w => "icons/Workflows/Planning|Icons/Validations/Edit")
            .Progress(0.5).Action(w => w.Target.Progress = 0.5)
            .WhenStageAllowed(() => MonographClosed)
        );

        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "{Put into production}").Icon(w => "Icons/Workflows/Production")
           .FromStage(() => Planning)
           .ToStage(() => Production)
           .NeedRight(() => AnalysisRights.AnalysisSchedule)
        );


        //########################################################
        // PRODUCTION
        public static Stage Production = Stage.Create(c => c
            .Caption(w => "{Production}").Icon(w => "Icons/Workflows/Production")
            .Progress(0.6).Action(w => w.Target.Progress = 0.6)
            .WhenStageAllowed(() => MonographClosed)
        );

        public static Action AskForMonographCorrection = Action.Create(c => c
           .Caption(w => "{Ask for Monograph correction}").Icon(w => "Icons/Workflows/Monograph|Icons/Workflows/Correct")
           .FromStage(() => Production)
           .ToStage(() => MonographReviewNeeded)
           .NeedRight(() => AnalysisRights.AnalysisResultEnter)
           .Sign().Motivate().Backward()
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create(c => c
           .Caption(w => "{Validate Certificate}").Icon(w => "Icons/Workflows/Certificate")
           .FromStage(() => Production)
           .Action(w =>
           {
               w.Target.NotificationDate = DateTime.Today;
               w.Target.Validator = WorkflowAnalysisExtension.Acl.Connection.User;
           })
           .ToStage(() => Certificate)
           .NeedRight(() => AnalysisRights.AnalysisCertificateCreate)
           .Sign()
        );

        public static Action AbortAnalysis = Action.Create(c => c
           .Caption(w => "{Abort analysis}").Icon(w => "Icons/Workflows/Aborted")
           .FromStage(() => Reception, () => Monograph, () => Planning, () => Production, () => Certificate, () => Closed)
           .Action(w =>
           {
               w.Target.Validator = WorkflowAnalysisExtension.Acl.Connection.User;
               w.Target.PreviousStageId = w.TargetStage.Name;
           })
           .ToStage(() => Aborted)
           .NeedRight(() => AnalysisRights.AnalysisCertificateCreate)
           .Sign().Motivate()
        );

        public static Stage Certificate = Stage.Create(c => c
            .Caption(w => "{Certificate}")
            .Icon(w => "Icons/Workflows/Certificate")
            .Progress(0.9).Action(w => w.Target.Progress = 0.9)
            .WhenStageAllowed(() => MonographClosed)
            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.ReportReference))
                .WithMessage(w => "{Missing} : {Certificate Reference}")
                .HighlightField(w => w.Target.ReportReference)
            .When(w =>
            {
                var validated = 0;
                var invalidated = 0;

                if (w._sampleTests == null) return false;

                foreach (SampleTest test in w._sampleTests)
                {
                    if (test.Stage == SampleTestWorkflow.ValidatedResults) validated++;
                    else if (test.Stage == SampleTestWorkflow.InvalidatedResults) invalidated++;
                    else return false;
                }

                return validated > 0;
            })
            .WithMessage(w => "{Some tests not validated yet}")
        );

        //########################################################
        // ABORTED

        public static Stage Aborted = Stage.Create(c => c
            .Caption(w => "{Aborted}").Icon(w => "Icons/Workflows/Aborted")
            .Progress(1.0)
            .Action(w => w.Target.Progress = 1.0)
        );

        public static Action ReopenAnalysis = Action.Create(c => c
           .Caption(w => "{Reopen analysis}").Icon(w => "Icons/Workflows/Redo")
           .FromStage(() => Aborted)
           .ToStage(w => StageFromName(w.Target.PreviousStageId))
           .NeedRight(() => AnalysisRights.AnalysisCertificateCreate)
           .Sign().Motivate()
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create(c => c
           .Caption(w => "{Close}").Icon(w => "Icons/Workflows/Close")
           .FromStage(() => Certificate)
           .ToStage(() => Closed)
           .NeedPharmacist()
           .Sign()
        );

        public static Stage Closed = Stage.Create(c => c
            .Caption(w => "{Closed}").Icon(w => "Icons/Workflows/Closed")
            .Progress(1.0).Action(w => w.Target.Progress = 1.0)
            .SetStage(() => Closed)
            //.When(w => w.Target.Invoiced).WithMessage(w => "{Not billed}")
            //.When(w => w.Target.Paid).WithMessage(w => "{Not Payed}")
            .WhenStageAllowed(() => Certificate)
        );
    }
}
