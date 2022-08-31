using System;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Base;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data.Entities;
using HLab.Erp.Lims.Analysis.Module.Filters;
using HLab.Erp.Workflows;
using HLab.Erp.Core.ListFilters;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests;

public static class ColumnsExtensions
{
    public static IColumnConfigurator<T,string,WorkflowFilter<TW>> StageColumn<T,TLink,TFilter,TW>(this IColumnConfigurator<T,TLink,TFilter> c,TW workflow, Expression<Func<T, string>> stageNameExpression) where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
        where TW : class,IWorkflow<TW>
    {
        var stageFromNameMethod = typeof(TW).GetMethod("StageFromName", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy/*,new []{typeof(string) }*/);
        IWorkflowStage stageFromName(string name) => (IWorkflowStage)stageFromNameMethod.Invoke(null,new []{name });
        var stageName = stageNameExpression.Compile();
        Func<T,IWorkflowStage> stage = e => stageFromName(stageName(e));

        return c.Column("Stage")
            .Header("{Stage}")
            .Width(180)
            .Localize(s => stage(s).GetCaption(null))
            .Icon(s => stage(s).GetIconPath(null), 20)
            .OrderBy(s => stage(s).Name)
            .Link(stageNameExpression)
            .Filter(default(WorkflowFilter<TW>))
            .Header("{Stage}")
            .IconPath("Icons/Workflow");
    }

    public static IColumnConfigurator<T,int?,EntityFilterNullable<Form>> FormColumn<T,TLink,TFilter>(this IColumnConfigurator<T,TLink,TFilter> c, Expression<Func<T, Form>> getForm) 
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var getter = getForm.Compile();

        return c.Column("Form")
            .Header("{Form}")
            .Width(160)
            .LinkNullable(getForm)
            .Content(e => getter(e))
            .Mvvm()
            .Filter();
    }


    public static IColumnConfigurator<T, ConformityState, IFilter<ConformityState>> Link<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, ConformityState>> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c.GetFilterConfigurator<ConformityState, IFilter<ConformityState>>();

        result.LinkExpression = getter;

        return result.UpdateOn(getter);
    }
    public static IColumnConfigurator<T, ConformityState, IFilter<ConformityState>> PostLink<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c, Func<T, ConformityState> getter)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var result = c.GetFilterConfigurator<ConformityState, IFilter<ConformityState>>();
        result.LinkLambda = getter;

        return result;
    }

    public static IColumnConfigurator<T,ConformityState,ConformityFilter> ConformityColumn<T, TLink, TFilter>(
        this IColumnConfigurator<T, TLink, TFilter> c, Expression<Func<T, ConformityState>> getStateExpression)
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var getState = getStateExpression.Compile();

        return c.Column("ConformityId")
                .Header("{Conformity}")
                .IconPath("Icons/Conformity/ConformHeader")
                .Width(130)
                .Link(getStateExpression)
                .Localize(s => $"{{{getState(s)}}}")
                .Icon(s => getState(s).IconPath(), 20)
                //.Center()
                .OrderBy(s => getState(s))
                .Filter(default(ConformityFilter))
            ;
    }
    public static IColumnConfigurator<T,ConformityState,ConformityFilter> ConformityColumnPostLinked<T, TLink, TFilter>(
        this IColumnConfigurator<T, TLink, TFilter> c, 
        Expression<Func<T, ConformityState>> getStateExpression
    )
        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var getState = getStateExpression.Compile();

        return c.Column("ConformityId")
                .Header("{Conformity}").Width(130)
                .IconPath("Icons/Conformity")
                .PostLink(getState)
                .Localize(s => $"{{{getState(s)}}}")
                .Icon(s => getState(s).IconPath(), 20)
                .Center()
                .OrderBy(s => getState(s))
                .Filter(default(ConformityFilter))
            ;
    }

    public static IColumnConfigurator<T, object, IFilter<object>> ProgressColumn<T, TLink, TFilter>(this IColumnConfigurator<T, TLink, TFilter> c,
        Expression<Func<T, double?>> getProgress)

        where T : class, IEntity, new()
        where TFilter : class, IFilter<TLink>
    {
        var getter = getProgress.Compile();
        return c.Column("Progress")
            .Header("{Progress}").Width(80)
            .AddProperty(getProgress,out var progress)
            .ContentTemplate($@"<{XamlTool.Type<ProgressView>()} Value=""{{Binding {progress}}}"" VerticalAlignment=""Stretch""/>")
            .UpdateOn(getProgress)
            .OrderBy(s => getter(s));
    }

    public static IColumnConfigurator<T, object, IFilter<object>> DescriptionColumn<T>(
        this IColumnConfigurator<T> @this,
        Expression<Func<T, object>> getTitle,
        Expression<Func<T, object>> getDescription,
        string id = null
    )

        where T : class, IEntity, new()
        => @this.Column(id)
            .AddProperty(getTitle, out var title)
            .AddProperty(getDescription, out var description)
            .ContentTemplate(@$"<{XamlTool.Type<ColumnDescriptionBlock>()} VerticalAlignment=""Top"" Title=""{{Binding {title}}}"" Description=""{{Binding {description}}}""/>");

    static string GetEntityName<T>() => typeof(T).Name;







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