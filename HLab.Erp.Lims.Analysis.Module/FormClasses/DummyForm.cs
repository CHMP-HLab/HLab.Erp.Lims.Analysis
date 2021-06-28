using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.TestClasses;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class DummyForm : UserControl, IForm
    {
        public IFormTarget Target { get; set; }

        public void Connect(int connectionId, object target){ }

        public void Process(object sender, RoutedEventArgs e){ }

        public IFormClassProvider FormClassProvider { get; set; }

        public FormMode Mode { get; set; }

        public string Version => "";

        public void SetFormMode(FormMode formMode)
        {
        }

        public void LoadValues(string values)
        {
        }

        public void SetErrorMessage(FrameworkElement fe)
        {
            Content = fe;
        }

        public void Upgrade(FormValues formValues)
        {
        }
    }
}