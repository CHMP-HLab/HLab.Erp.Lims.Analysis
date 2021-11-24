using System;
using System.Collections.Generic;
using System.Windows;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Lims.Analysis.Module.TestClasses;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public interface IForm
    {
        IFormTarget Target { get; set; }

        void Connect(int connectionId, object target) { }
        void Process(object sender, RoutedEventArgs e) { }

        FormMode Mode { get; set; }
        string Version => string.Empty;

        IEnumerable<FrameworkElement> NamedElements {get; set;}

        void SetFormMode(FormMode formMode);

        void LoadValues(string values);

        bool PreventProcess();
        void AllowProcess();

        void SetErrorMessage(FrameworkElement fe);

        public void TryProcess(object sender, RoutedEventArgs args);

        void Upgrade(FormValues formValues) { }
    }
}