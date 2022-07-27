using System;
using System.Text;
using HLab.Base.Fluent;
using HLab.Core.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Workflows;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Data.Workflows
{
    public class WorkFlowBootloader : IBootloader
    {
        readonly IAclService _acl;

        public WorkFlowBootloader(IAclService acl)
        {
            _acl = acl;
        }

        public void Load(IBootContext bootstrapper)
        {
            WorkflowAnalysisExtension.Acl = _acl;
        }
    }

    public static class WorkflowAnalysisExtension
    {
        public static IAclService Acl { get; set; }

        /// <summary>
        /// Require user with specific right
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedRight<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, Func<AclRight> right)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.When(w => Acl.IsGranted(
                    right()
                    //,w.User,w.Target
                ))
                .WithMessage(w => "{Not allowed} {need} " + right().Caption);
        }

        /// <summary>
        /// Require user with almost one right of a list
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <param name="rights">List of rights from one only is required</param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedAnyRight<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t, params Func<AclRight>[] rights)
            where TWf : NotifierBase, IWorkflow<TWf> => t.When(w =>
                                                                  {
                                                                      foreach (var right in rights)
                                                                          if (Acl.IsGranted(right()
                                                                              //, w.User, w.Target
                                                                              )) return true;
                                                                      return false;
                                                                  })
            .WithMessage(w =>
            {
                var s = new StringBuilder("{Not allowed} {need} ");
                foreach (var right in rights) s.Append(right().Caption).Append(" ");
                return  s.ToString();
            });

        /// <summary>
        /// Require a user that have <see cref="AnalysisRights.AnalysisCertificateCreate"/> right
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPharmacist<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return t.NotWhen(w => !Acl.IsGranted(
                    AnalysisRights.AnalysisCertificateCreate
                    //, w.User,w.Target
                    ))
                .WithMessage(w => "{Pharmacist needed}");
        }

        /// <summary>
        /// Require a user that have <see cref="AnalysisRights.AnalysisResultValidate"/> right
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedValidator<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> t)
            where TWf : Workflow<TWf>
        {
            return t.NotWhen(w => !Acl.IsGranted(AnalysisRights.AnalysisResultValidate
                //,w.User,w.Target
                ))
                .WithMessage(w => "validator needed");
        }

        /// <summary>
        /// Require a user that have <see cref="AnalysisRights.AnalysisSchedule"/> right
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> NeedPlanner<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return c.NotWhen(w => !Acl.IsGranted(AnalysisRights.AnalysisSchedule
                //,w.User,w.Target
                ))
                .WithMessage(w => "planner needed");
        }

        /// <summary>
        /// Move workflow to specific stage
        /// </summary>
        /// <typeparam name="TWf"></typeparam>
        /// <param name="c"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFluentConfigurator<IWorkflowConditionalObject<TWf>> 
            SetStage<TWf>(this IFluentConfigurator<IWorkflowConditionalObject<TWf>> c, Func<Workflow<TWf>.Stage> getter)
            where TWf : NotifierBase, IWorkflow<TWf>
        {
            return c
                .Action(async w =>
                {
                    await w.SetStageAsync(getter,"","",false,false);
                });
        }
    }
}