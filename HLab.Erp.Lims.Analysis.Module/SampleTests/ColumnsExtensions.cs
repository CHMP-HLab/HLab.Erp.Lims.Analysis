using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Lims.Analysis.Data;
using HLab.Erp.Workflows;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    public static class ColumnsExtensions
    {
        public static IColumnConfigurator<T> StageColumn<T>(this IColumnConfigurator<T> c, Func<T, IWorkflowStage> getStage)
            => c.Column
                .Header("{Stage}").Width(180)
                .Content(s => getStage(s).GetCaption(null))
                .Localize()
                .Icon(s => getStage(s)?.GetIconPath(null),20)
                .OrderBy(s => getStage(s).Name);

        public static IColumnConfigurator<T> FormColumn<T>(this IColumnConfigurator<T> c, Func<T, Form> getForm)
            => c.Column
                .Header("{Form}").Width(160)
                .Content(e => getForm(e).Name)
                .Localize()
                .Icon(s => getForm(s)?.IconPath ?? "")
                .OrderBy( s => getForm(s).Name);
        private static string GetIcon(int? state)
        {
            return state switch
            {
                1 => "icons/Results/ConformityTodo",
                2 => "icons/Results/ConformityKo",
                3 => "icons/Results/ConformityOk",
                _ => "icons/Results/ConformityTodo"
            };
        }

        public static IColumnConfigurator<T> ConformityColumn<T>(this IColumnConfigurator<T> c, Func<T, int?> getState)
            => c.Column
                .Header("{Conformity}").Width(80)
                .Icon(s => GetIcon(getState(s)))
                .Center()
                .OrderBy(s => getState(s));

        public static IColumnConfigurator<T> ProgressColumn<T>(this IColumnConfigurator<T> c,
            Func<T, double?> getProgress)
            => c.Column
                .Header("{Progress}").Width(80)
                .Content(s => new ProgressViewModel {Value = getProgress(s) ?? 0})
                .OrderBy(s => getProgress(s));

        public static IColumnConfigurator<T> DescriptionColumn<T>(this IColumnConfigurator<T> c,
            Func<T, string> getTitle, Func<T, string> getDescription)
            => c.Column
                .Content(s => new ColumnDescriptionBlock()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Title = getTitle(s),
                    Description = getDescription(s)
                });

    }
}
