using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module
{
    public class SampleWorkflow : Workflow<SampleWorkflow,Sample>
    {


        public SampleWorkflow(Sample sample):base(sample)
        {
            H.Initialize(this);

            CurrentState = Reception;
        }

        public override void OnSetState(State state)
        {
            Target.Stage = state.Name;
        }

        //########################################################
        // RECEPTION

        public static State Reception = State.Create(c => c
            .Caption("{Reception entry}").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Reception)
        );

        public static State ReceptionClosed = State.Create(c => c
            .Caption("{Reception check}").Icon("Icons/Workflow/ReceptionCheck")
            .NotWhen(w => w.Target.CustomerId == null)
            .WithMessage(w => "{Missing customer}")

            .NotWhen(w => w.Target.ProductId == null)
            .WithMessage(w => "{Missing Product}")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Batch))
            .WithMessage(w => "{Missing batch}")

            .NotWhen(w => w.Target.ReceptionDate == null)
            .WithMessage(w => "{Missing reception date}")

            .NotWhen(w => w.Target.ReceivedQuantity == null)
            .WithMessage(w => "{Missing received quantity}")
        );

        public static Action CloseReception = Action.Create(c => c
            .Caption("Close").Icon("icons/workflow/CloseReception")
            .FromState(() => Reception)
            .ToState(() => ReceptionClosed)
            .NeedRight(AnalysisRights.AnalysisReceptionValidate)
        );

        public static Action ValidateReception = Action.Create(c => c
            .Caption(w => "Check").Icon(w => "icons/workflow/ReceptionChecked")
            .FromState(() => ReceptionClosed)
            .ToState(() => Monograph)
            .NeedValidator()
        );

        //########################################################
        // MONOGRAPH

        public static State Monograph = State.Create(c => c
            .Caption(w => "Monograph Entry").Icon(w => "Icons/Sample/PackageOpened")
            .WhenStateAllowed(() => ReceptionClosed)
            .SetState(() => Monograph)
        );

        public static Action ValidateMonograph = Action.Create(c => c
           .Caption(w => "Validate monograph").Icon(w => "icons/workflow/MonographEdit")
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
           .NeedValidator()
            );

        public static State MonographClosed = State.Create(c => c
            .Caption(w => "Monograph validated").Icon(w => "icons/workflow/MonographCheck")
            .SetState(() => MonographClosed)

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "Missing pharmacopoeia")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "Missing pharmacopoeia version")

            .NotWhen(w => w.Target.SampleTests.Count == 0)
                .WithMessage(w => "Missing tests")

            .NotWhen(w => {
                foreach (SampleTest test in w.Target.SampleTests)
                    if (test.Stage < 1) return true; // TODO
                return false;
            }).WithMessage(w => "Some tests Missing specifications")
        );

        //########################################################
        // PLANNING
        public static Action ValidatePanning = Action.Create(c => c
           .Caption(w => "Planifier").Icon(w => "icons/workflow/Planning")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedRight(AnalysisRights.AnalysisSchedule)
        );

        public static State Planning = State.Create(c => c
            .Caption(w => "Plannification").Icon(w => "icons/workflow/PlanningEdit")
            .WhenStateAllowed(() => MonographClosed)
            .SetState(() => Planning)
        );

        //########################################################
        // PRODUCTION
        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "Mettre en production").Icon(w => "icons/workflow/Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedRight(AnalysisRights.AnalysisSchedule)
        );

        public static State Production = State.Create(c => c
            .Caption(w => "En Production").Icon(w => "icons/workflow/Production")
            .SetState(()=>Production)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create( c => c
            .Caption(w => "Print Certificate").Icon(w => "icons/workflow/Certificate")
            .FromState(() => Planning)
            .ToState(() => Production)
            .NeedRight(AnalysisRights.AnalysisSchedule)
        );

        public static State Certificate = State.Create(c => c
            .Caption(w => "Edition du certificate").Icon(w => "icons/workflow/Certificate")
            .SetState(()=>Certificate)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create( c => c
            .Caption(w => "Close").Icon(w => "icons/workflow/Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
        );

        public static State Closed = State.Create(c => c
            .Caption(w => "Closed").Icon(w => "icons/workflow/Closed")
            .SetState(() => Closed)
            .NotWhen(w => !w.Target.Invoiced).WithMessage(w => "N'est pas facturé")
            .NotWhen(w => !w.Target.Paid).WithMessage(w => "N'est pas payé")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
