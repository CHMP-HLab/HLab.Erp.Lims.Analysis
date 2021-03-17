using System.Windows;
using System.Windows.Controls;
using HLab.Base;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.TestClasses
{
    public class DummyTestForm : UserControl, ITestForm
    {
        public ITestHelper Test => null;

        public void Connect(int connectionId, object target)
        {
        }

        public void Traitement(object sender, RoutedEventArgs e)
        {
        }
    }
}
