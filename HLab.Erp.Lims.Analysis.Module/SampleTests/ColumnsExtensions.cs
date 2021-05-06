using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Filters;
using HLab.Erp.Workflows;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public static class ColumnsExtensions
    {
        public static IColumnConfigurator<T,string,WorkflowFilter<TW>> StageColumn<T,TLink,TFilter,TW>(this IColumnConfigurator<T,TLink,TFilter> c,TW workflow, Expression<Func<T, string>> stageNameExpression) where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
            where TW : class,IWorkflow<TW>
        {
            var stageFromNameMethod = typeof(TW).GetMethod("StageFromName", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy/*,new []{typeof(string) }*/);
            IWorkflowStage stageFromName(string name) => (IWorkflowStage)stageFromNameMethod.Invoke(null,new []{name });
            var stageName = stageNameExpression.Compile();
            Func<T,IWorkflowStage> stage = e => stageFromName(stageName(e));

            return c.Column()
                .Header("{Stage}").Width(180).Content(s => stage(s).GetCaption(null))
                .Localize()
                .Icon(s => stage(s)?.GetIconPath(null), 20)
                .OrderBy(s => stage(s).Name)
                .Link(stageNameExpression)
                .Filter(default(WorkflowFilter<TW>))
                .Header("{Stage}")
                .IconPath("Icons/Workflow");
        }

        public static IColumnConfigurator<T,int?,EntityFilterNullable<Form>> FormColumn<T,TLink,TFilter>(this IColumnConfigurator<T,TLink,TFilter> c, Expression<Func<T, Form>> getForm) 
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var getter = getForm.Compile();

            return c.Column()
                .Header("{Form}")
                .Width(160)
                .LinkNullable(getForm)
                .Content(e => getter(e))
                .Mvvm()
                .Filter();
        }


        public static IColumnConfigurator<T, ConformityState?, IFilter<ConformityState?>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, ConformityState?>> getter)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var result = c.GetChildConfigurator<ConformityState?, IFilter<ConformityState?>>();
            result.Link = getter;

            return result;
        }

        public static IColumnConfigurator<T,ConformityState?,ConformityFilter> ConformityColumn<T, TLink, TFilter>(
            this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, ConformityState?>> getStateExpression)
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
        {
            var getState = getStateExpression.Compile();

            return c.Column()
                .Header("{Conformity}").Width(130)
                .Link(getStateExpression)
                .Content(s => $"{{{getState(s)}}}").Localize()
                .Icon(s => (getState(s) ?? ConformityState.Undefined).IconPath(), 20)
                .Center()
                .OrderBy(s => getState(s))
                .Filter(default(ConformityFilter))
                .IconPath("Icons/Conformity");
        }

        public static IColumnConfigurator<T, object, IFilter<object>> ProgressColumn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c,
            Func<T, double?> getProgress) 
            
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
            => c.Column()
                .Header("{Progress}").Width(80)
                .Content(s => new ProgressViewModel {Value = getProgress(s) ?? 0})
                .OrderBy(s => getProgress(s));

        public static IColumnConfigurator<T, object, IFilter<object>> DescriptionColumn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c,
            Func<T, string> getTitle, Func<T, string> getDescription) 
            
            where T : class, IEntity, new()
            where TFilter : IFilter<TLink>
            => c.Column()
                .Content(s => new ColumnDescriptionBlock()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Title = getTitle(s),
                    Description = getDescription(s)
                });

        private static string GetEntityName<T>() => typeof(T).Name;







        //public static IColumnConfigurator<T, int?, TFilter> PostLinkedColumn<T, TE, TFilter>(this IColumnConfigurator<T, int?, TFilter> c,
            
        //    Func<T, TE> getter, 
        //    Func<T,int?> getterId) 
        //    where T : class, IEntity, new()
        //    where TE :  class, IListableModel, IEntity<int>, new()
        //{

        //    return c.Column()
        //            .Header($"{{{typeof(TE).Name}}}")
        //            .Link(getter)
        //        //    .Content(e => getter(e)==null?"":getter(e).Caption)
        //        //.Icon(e => getter(e)?.IconPath)
        //        //.OrderBy(e => getter(e).Caption)
        //        ////.Link(GetIdExpression<T, TE>(getterId))
        //        //.PostLink(getterId);
        //        //.Filter()
        //        ;
        //}
        //public static IColumnConfigurator<T, int, TFilter> PostLinkedColumn<T, TE, TFilter>(this IColumnConfigurator<T, int, TFilter> c,
        //    Func<T, TE> getter, Func<T,int> getterId) 
        //    where T : class, IEntity, new()
        //    where TE :  class, IListableModel, IEntity<int?>, new()
        //{

        //    return c
        //        .Column()
        //        .Header($"{{{typeof(TE).Name}}}")
        //        .Content(e => getter(e).Caption)
        //        .Icon(e => getter(e).IconPath)
        //        .OrderBy(e => getter(e).Caption)
        //        //.Link(GetIdExpression<T, TE>(getterId))
        //        .PostLink(getterId)
        //        .Filter();
        //}
    }
}
