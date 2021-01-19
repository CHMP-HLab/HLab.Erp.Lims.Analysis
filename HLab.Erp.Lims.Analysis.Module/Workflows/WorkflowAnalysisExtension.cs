using System;
using System.Text;
using HLab.Base.Fluent;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Lims.Analysis.Module.Workflows;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;
using NPoco.Expressions;

namespace HLab.Erp.Lims.Analysis.Module
{
    static class WorkflowAnalysisExtension
    {
        [Import]
        public static IAclService Acl { get; set; }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedRight<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<AclRight> right)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.When(w => Acl.IsGranted(
                    right(),
                    w.User,w.Target))
                .WithMessage(w => "{Not allowed} {need} " + right().Caption);
        }
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedAnyRight<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, params Func<AclRight>[] rights)
            where TWf : NotifierBase, IWorkflow<TWf> => t.When(w =>
                                                                  {
                                                                      foreach (var right in rights)
                                                                          if (Acl.IsGranted(right(), w.User, w.Target)) return true;
                                                                      return false;
                                                                  })
            .WithMessage(w =>
            {
                var s = new StringBuilder("{Not allowed} {need} ");
                foreach (var right in rights) s.Append(right().Caption).Append(" ");
                return  s.ToString();
            });

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPharmacist<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.NotWhen(w => !Acl.IsGranted(
                    AnalysisRights.AnalysisCertificateCreate,
                    w.User,w.Target))
                .WithMessage(w => "{Pharmacist needed}");
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedValidator<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : Workflow<TWf>
        {
            return t.NotWhen(w => !Acl.IsGranted(AnalysisRights.AnalysisResultValidate
                ,w.User,w.Target))
                .WithMessage(w => "validator needed");
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPlanner<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.NotWhen(w => !Acl.IsGranted(AnalysisRights.AnalysisSchedule
                ,w.User,w.Target))
                .WithMessage(w => "planner needed");
        }

        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            SetState<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<Workflow<TWf>.State> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t
                .Action(async w =>
                {
                    await w.SetStateAsync(getter);
                });
        }
    }
}