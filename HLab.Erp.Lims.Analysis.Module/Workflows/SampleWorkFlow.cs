using HLab.Erp.Workflows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Base.Fluent;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Acl;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.Workflows
{
    static class AnalysisRights
    {
        public static readonly AclRight AnalysisCertificateCreate = new AclRight();
        public static readonly AclRight AnalysisResultValidate = new AclRight();
        public static readonly AclRight AnalysisResultEnter = new AclRight();
        public static readonly AclRight AnalysisSchedule = new AclRight();
    }

    static class WorkflowAnalysisExtension
    {
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPharmacist<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.NotWhen(w => !w.User.HasRight(AnalysisRights.AnalysisCertificateCreate))
                .WithMessage(w => "Pharmacist needed");
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedValidator<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : Workflow<TWf>
        {
            return t.NotWhen(w => !w.User.HasRight(AnalysisRights.AnalysisResultValidate))
                .WithMessage(w => "Pharmacist or validator needed");
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPlanner<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.NotWhen(w => !w.User.HasRight(AnalysisRights.AnalysisSchedule))
                .WithMessage(w => "Pharmacist or planner needed");
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            SetState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getter)

            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t
                //.NotWhen(w => !(w as SampleWorkflow).Sample.Lock.Actif)
                .Action(w =>
                {
                    w.SetState(getter);
                });
        }

        //public static TC SetState<TC>(this TC t, state state)
        //    where TC : IConfigurator<SampleWorkflow>
        //    => t.SetState<TC>(() => state);
    }


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
            .Caption("^Reception entry").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Reception)
        );

        public static State ReceptionClosed = State.Create(c => c
            .Caption("Reception check").Icon("ReceptionCheck")
            .NotWhen(w => w.Target.CustomerId == null)
            .WithMessage(w => "Missing customer")

            .NotWhen(w => w.Target.ProductId == null)
            .WithMessage(w => "Missing Product")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.Batch))
            .WithMessage(w => "Missing batch")

            .NotWhen(w => w.Target.ReceptionDate == null)
            .WithMessage(w => "Missing reception date")

            .NotWhen(w => w.Target.ReceivedQuantity == null)
            .WithMessage(w => "Missing received quantity")
        );

        public static Action CloseReception = Action.Create(c => c
            .Caption("Close").Icon("CloseReception")
            .FromState(() => Reception)
            .ToState(() => ReceptionClosed)
            .NeedValidator()
        );

        public static Action ValidateReception = Action.Create(c => c
            .Caption(w => "Check").Icon(w => "ReceptionChecked")
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
           .Caption(w => "Validate monograph").Icon(w => "MonographEdit")
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
           .NeedValidator()
            );

        public static State MonographClosed = State.Create(c => c
            .Caption(w => "Monograph validated").Icon(w => "MonographCheck")
            .SetState(() => MonographClosed)

            .NotWhen(w => w.Target.PharmacopoeiaId == null)
                .WithMessage(w => "Missing pharmacopoeia")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Target.PharmacopoeiaVersion))
                .WithMessage(w => "Missing pharmacopoeia version")

            .NotWhen(w => w.Target.SampleAssays.Count == 0)
                .WithMessage(w => "Missing assays")

            .NotWhen(w => {
                foreach (SampleAssay test in w.Target.SampleAssays)
                    if (test.Stage < 1) return true; // TODO
                return false;
            }).WithMessage(w => "Some tests Missing specifications")
        );

        //########################################################
        // PLANNING
        public static Action ValidatePanning = Action.Create(c => c
           .Caption(w => "Planifier").Icon(w => "Planning")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedPlanner()
            );

        public static State Planning = State.Create(c => c
            .Caption(w => "Plannification").Icon(w => "PlanningEdit")
            .WhenStateAllowed(() => MonographClosed)
            .SetState(() => Planning)
        );

        //########################################################
        // PRODUCTION
        public static Action ValidateProduction = Action.Create(c => c
           .Caption(w => "Mettre en production").Icon(w => "Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedPlanner()
            );

        public static State Production = State.Create(c => c
            .Caption(w => "En Production").Icon(w => "Production")
            .SetState(()=>Production)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CERTIFICAT

        public static Action ValidateCertificate = Action.Create( c => c
            .Caption(w => "Print Certificate").Icon(w => "Certificate")
            .FromState(() => Planning)
            .ToState(() => Production)
            .NeedPlanner()
            );

        public static State Certificate = State.Create(c => c
            .Caption(w => "Edition du certificate").Icon(w => "Certificate")
            .SetState(()=>Certificate)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CLOSE

        public static Action Close = Action.Create( c => c
            .Caption(w => "Close").Icon(w => "Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
        );

        public static State Closed = State.Create(c => c
            .Caption(w => "Closed").Icon(w => "Closed")
            .SetState(() => Closed)
            .NotWhen(w => !w.Target.Invoiced).WithMessage(w => "N'est pas facturé")
            .NotWhen(w => !w.Target.Paid).WithMessage(w => "N'est pas payé")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
