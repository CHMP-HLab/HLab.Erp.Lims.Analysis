using System.Windows;
using HLab.Erp.Conformity.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public interface IForm
    {
        IFormTarget Target { get; set; }

        void Connect(int connectionId, object target);
        void Process(object sender, RoutedEventArgs e);
    }
}