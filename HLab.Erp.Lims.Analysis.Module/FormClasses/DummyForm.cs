using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Forms.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class DummyForm : UserControl, IForm
    {
        public IFormTarget Target { get; set; }

        public void Connect(int connectionId, object target)
        { }

        public void Process(object sender, RoutedEventArgs e)
        { }
    }
}