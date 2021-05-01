using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Erp.Data;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Lims.Analysis.Module.Filters;
using HLab.Erp.Workflows;
using HLab.Mvvm.Application;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Expression = System.Linq.Expressions.Expression;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public static class ColumnsExtensions
    {
        public static IFilterConfigurator<T> StageColumn<T,TW>(this IListConfigurator<T> c,TW workflow, Expression<Func<T, string>> stageNameExpression) where T : class, IEntity, new()
        where TW : class,IWorkflow<TW>
        {
            var stageFormeNameMethod = typeof(TW).GetMethod("StageFromName", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy/*,new []{typeof(string) }*/);
            IWorkflowStage stageFromName(string name) => (IWorkflowStage)stageFormeNameMethod.Invoke(null,new []{name });
            var stageName = stageNameExpression.Compile();
            Func<T,IWorkflowStage> stage = e => stageFromName(stageName(e));

            return c.Column()
                           .Header("{Stage}").Width(180)
                           .Content(s => stage(s).GetCaption(null))
                           .Localize()
                           .Icon(s => stage(s)?.GetIconPath(null), 20)
                           .OrderBy(s => stage(s).Name)
                               .Filter<WorkflowFilter<TW>>()
                               .Header("{Stage}")
                               .IconPath("Icons/Workflow")
                               .Link(stageNameExpression);
        }

        public static IFilterConfigurator<T> FormColumn<T>(this IListConfigurator<T> c, Func<T, Form> getForm, Expression<Func<T, int?>> getId) where T : class, IEntity, new()
            => c.Column()
                .Header("{Form}").Width(160)
                .Content(e => getForm(e)?.Name)
                .Localize()
                .Icon(s => getForm(s)?.IconPath ?? "")
                .OrderBy( s => getForm(s)?.Name??"")

                .Filter<EntityFilterNullable<Form>>()
                    .Link(getId)
            ;

        public static IFilterConfigurator<T> ConformityColumn<T>(this IListConfigurator<T> c, Func<T, ConformityState?> getState) where T : class, IEntity, new()
            => c.Column()
                .Header("{Conformity}").Width(130)
                .Content(s => $"{{{getState(s)}}}").Localize()
                .Icon(s => (getState(s)??ConformityState.Undefined).IconPath(),20)
                .Center()
                .OrderBy(s => getState(s))
            
                    .Filter<ConformityFilter>()
                        .Header("{Conformity}")
                        .IconPath("Icons/Conformity")
                        .PostLink(s => getState(s)??ConformityState.Undefined);

        public static IColumnConfigurator<T> ProgressColumn<T>(this IListConfigurator<T> c,
            Func<T, double?> getProgress) where T : class, IEntity, new()
            => c.Column()
                .Header("{Progress}").Width(80)
                .Content(s => new ProgressViewModel {Value = getProgress(s) ?? 0})
                .OrderBy(s => getProgress(s));

        public static IColumnConfigurator<T> DescriptionColumn<T>(this IListConfigurator<T> c,
            Func<T, string> getTitle, Func<T, string> getDescription) where T : class, IEntity, new()
            => c.Column()
                .Content(s => new ColumnDescriptionBlock()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Title = getTitle(s),
                    Description = getDescription(s)
                });

        private static string GetEntityName<T>() => typeof(T).Name;


        private static readonly MethodInfo GetIdMethod = typeof(IEntity<int>).GetProperty("Id").GetMethod;

        private static Expression<Func<T,int>> GetIdExpression<T,TE>(Expression<Func<T,TE>> getter)
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int>, new()
        {
            var entity = getter.Parameters[0];
            var ex = Expression.Call(getter.Body, GetIdMethod);
            return Expression.Lambda<Func<T, int>>(ex, entity);
        }

        public static IListConfigurator<T> Column<T,TE>(this IListConfigurator<T> c,
            Func<T, TE> getter, Expression<Func<T,int>> getterId) 
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int>, new()
        {

            return c.Column()
                .Header($"{{{typeof(TE).Name}}}")
                .Content(e => getter(e).Caption)
                .Icon(e => getter(e).IconPath)
                .OrderBy(e => getter(e).Caption)
                .Link(getterId)
                //.Link(GetIdExpression<T, TE>(getterId))
                .EntityFilter<TE>();
        }

        private static Expression<Func<T, int?>> GetterIdFromGetter<T,TE>(Expression<Func<T, TE>> getter)
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int>, new()
        {
            if (getter.Body is MemberExpression member)
            {
                var name = member.Member.Name;
                var method = typeof(T).GetProperty($"{name}Id")?.GetMethod;

                var entity = Expression.Parameter(typeof(T),"e");
                if (method != null)
                {
                    var property = Expression.Property(entity, method);
                    return Expression.Lambda<Func<T, int?>>(property, entity);
                }
            }

            return t => -1;
        }


        public static IListConfigurator<T> Column<T,TE>(this IListConfigurator<T> c,
            Expression<Func<T, TE>> getter, 
            Expression<Func<T,int?>> getterId = null,
            double width = double.NaN
            ) 
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int>, new()
        {
            var lambda = getter.Compile();
            getterId ??= GetterIdFromGetter(getter);

            return c.Column()
                .Header($"{{{typeof(TE).Name}}}")
                .Width(width)
                .Content(e => lambda(e)?.Caption)
                .Icon(e => lambda(e)?.IconPath)
                .OrderBy(e => lambda(e)?.Caption)
                .Link(getterId)
                .EntityFilter<TE>();
        }
        public static IListConfigurator<T> PostLinkedColumn<T,TE>(this IListConfigurator<T> c,
            Func<T, TE> getter, Func<T,int?> getterId) 
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int>, new()
        {

            return c.Column()
                .Header($"{{{typeof(TE).Name}}}")
                .Content(e => getter(e)?.Caption)
                .Icon(e => getter(e)?.IconPath)
                .OrderBy(e => getter(e).Caption)
                //.Link(GetIdExpression<T, TE>(getterId))
                .Filter<EntityFilterNullable<TE>>()
                .PostLink(getterId);
        }
        public static IListConfigurator<T> PostLinkedColumn<T,TE>(this IListConfigurator<T> c,
            Func<T, TE> getter, Func<T,int> getterId) 
            where T : class, IEntity, new()
            where TE :  class, IListableModel, IEntity<int?>, new()
        {

            return c.Column()
                .Header($"{{{typeof(TE).Name}}}")
                .Content(e => getter(e).Caption)
                .Icon(e => getter(e).IconPath)
                .OrderBy(e => getter(e).Caption)
                //.Link(GetIdExpression<T, TE>(getterId))
                .Filter<EntityFilter<TE>>()
                .PostLink(getterId);
        }
    }
}
