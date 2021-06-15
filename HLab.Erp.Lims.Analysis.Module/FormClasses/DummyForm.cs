﻿using System.Windows;
using System.Windows.Controls;
using HLab.Erp.Conformity.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    public class DummyForm : UserControl, IForm
    {
        public IFormTarget Target { get; set; }

        public void Connect(int connectionId, object target){ }

        public void Process(object sender, RoutedEventArgs e){ }

        public IFormClassProvider FormClassProvider { get; set; }

        public FormMode Mode { get; set; }
        public void SetFormMode(FormMode formMode)
        {
        }

        public void LoadValues(string values)
        {
        }
    }
}