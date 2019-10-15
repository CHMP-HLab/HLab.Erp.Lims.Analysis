using HLab.Erp.Workflows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Base.Fluent;
using HLab.Erp.Lims.Analysis.Data;
using state = HLab.Erp.Workflows.WorkflowState<HLab.Erp.Lims.Analysis.Module.Workflows.SampleWorkflow>;
using action = HLab.Erp.Workflows.WorkflowActionClass<HLab.Erp.Lims.Analysis.Module.Workflows.SampleWorkflow>;
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
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> SetState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<state> getter)
            where TWf : SampleWorkflow, IWorkflow<TWf>
        {
            return t
                //.NotWhen(w => !(w as SampleWorkflow).Sample.Lock.Actif)
                .Action(w =>
                {
                    w.Sample.Stage = getter().Name;
                });
        }

        //public static TC SetState<TC>(this TC t, state state)
        //    where TC : IConfigurator<SampleWorkflow>
        //    => t.SetState<TC>(() => state);
    }


    public class SampleWorkflow : Workflow<SampleWorkflow>
    {


        public SampleWorkflow(Sample sample)
        {
            Sample = sample;

            H.Initialize(this);

            State = Reception;
            Update();
            sample.PropertyChanged += Sample_PropertyChanged;
        }

        private void Sample_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Update();
        }

        public Sample Sample { get; }

        //########################################################
        // RECEPTION

        public static state Reception = state.Create(c => c
            .Caption("^Reception entry").Icon("Icons/Sample/PackageOpened")
            .SetState(() => Reception)
        );

        public static state ReceptionClosed = state.Create(c => c
            .Caption("Reception check").Icon("ReceptionCheck")
            .NotWhen(w => w.Sample.CustomerId == null)
            .WithMessage(w => "Missing customer")

            .NotWhen(w => w.Sample.ProductId == null)
            .WithMessage(w => "Missing Product")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Sample.Batch))
            .WithMessage(w => "Missing batch")

            .NotWhen(w => w.Sample.ReceptionDate == null)
            .WithMessage(w => "Missing reception date")

            .NotWhen(w => w.Sample.ReceivedQuantity == null)
            .WithMessage(w => "Missing received quantity")
        );

        public static action CloseReception = action.Create(c => c
            .Caption("Close").Icon("CloseReception")
            .FromState(() => Reception)
            .ToState(() => ReceptionClosed)
            .NeedValidator()
        );

        public static action ValidateReception = action.Create(c => c
            .Caption(w => "Check").Icon(w => "ReceptionChecked")
            .FromState(() => ReceptionClosed)
            .ToState(() => Monograph)
            .NeedValidator()
        );

        //########################################################
        // MONOGRAPH

        public static state Monograph = state.Create(c => c
            .Caption(w => "Monograph Entry").Icon(w => "Icons/Sample/PackageOpened")
            .WhenStateAllowed(() => ReceptionClosed)
            .SetState(() => Monograph)
        );

        public static action ValidateMonograph = action.Create(c => c
           .Caption(w => "Validate monograph").Icon(w => "MonographEdit")
           .FromState(() => Monograph)
           .ToState(() => MonographClosed)
           .NeedValidator()
            );

        public static state MonographClosed = state.Create(c => c
            .Caption(w => "Monograph validated").Icon(w => "MonographCheck")
            .SetState(() => MonographClosed)

            .NotWhen(w => w.Sample.PharmacopoeiaId == null)
                .WithMessage(w => "Missing pharmacopoeia")

            .NotWhen(w => string.IsNullOrWhiteSpace(w.Sample.PharmacopoeiaVersion))
                .WithMessage(w => "Missing pharmacopoeia version")

            .NotWhen(w => w.Sample.SampleAssays.Count == 0)
                .WithMessage(w => "Missing assays")

            .NotWhen(w => {
                foreach (SampleAssay test in w.Sample.SampleAssays)
                    if (test.Stage < 1) return true; // TODO
                return false;
            }).WithMessage(w => "Some tests Missing specifications")
        );

        //########################################################
        // PLANNING
        public static action ValidatePanning = action.Create(c => c
           .Caption(w => "Planifier").Icon(w => "Planning")
           .FromState(() => MonographClosed)
           .ToState(() => Planning)
           .NeedPlanner()
            );

        public static state Planning = state.Create(c => c
            .Caption(w => "Plannification").Icon(w => "PlanningEdit")
            .WhenStateAllowed(() => MonographClosed)
            .SetState(() => Planning)
        );

        //########################################################
        // PRODUCTION
        public static action ValidateProduction = action.Create(c => c
           .Caption(w => "Mettre en production").Icon(w => "Production")
           .FromState(() => Planning)
           .ToState(() => Production)
           .NeedPlanner()
            );

        public static state Production = state.Create(c => c
            .Caption(w => "En Production").Icon(w => "Production")
            .SetState(()=>Production)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CERTIFICAT

        public static action ValidateCertificate = action.Create( c => c
            .Caption(w => "Print Certificate").Icon(w => "Certificate")
            .FromState(() => Planning)
            .ToState(() => Production)
            .NeedPlanner()
            );

        public static state Certificate = state.Create(c => c
            .Caption(w => "Edition du certificate").Icon(w => "Certificate")
            .SetState(()=>Certificate)
            .WhenStateAllowed(() => Planning)
        );

        //########################################################
        // CLOSE

        public static action Close = action.Create( c => c
            .Caption(w => "Close").Icon(w => "Close")
            .FromState(() => Certificate)
            .ToState(() => Closed)
            .NeedPharmacist()
        );

        public static state Closed = state.Create(c => c
            .Caption(w => "Closed").Icon(w => "Closed")
            .SetState(() => Closed)
            .NotWhen(w => !w.Sample.Invoiced).WithMessage(w => "N'est pas facturé")
            .NotWhen(w => !w.Sample.Paid).WithMessage(w => "N'est pas payé")
            .WhenStateAllowed(() => Certificate)
        );
    }
}
