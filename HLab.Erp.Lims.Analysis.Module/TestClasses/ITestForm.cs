using System.Windows;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public interface ITestForm
    {
        void Connect(int connectionId, object target);
        void Traitement(object sender, RoutedEventArgs e);

        ITestHelper Test { get; }
    }
}