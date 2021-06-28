using System;
using System.Windows;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.TestClasses;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public interface IForm
    {
        IFormTarget Target { get; set; }

        void Connect(int connectionId, object target);
        void Process(object sender, RoutedEventArgs e);

        FormMode Mode { get; set; }
        string Version { get; }

        void SetFormMode(FormMode formMode);

        void LoadValues(string values);

        void SetErrorMessage(FrameworkElement fe);

        public void TryProcess(object sender, RoutedEventArgs args)
        {
            try
            {
                Process(sender, args);
                SetFormMode(Mode);
            }
            catch (Exception e)
            {
                SetErrorMessage(new ExceptionView { DataContext = e });
            }

        }

        void Upgrade(FormValues formValues);
    }
}