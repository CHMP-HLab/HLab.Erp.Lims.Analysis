using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HLab.Erp.Core;
using HLab.Erp.Lims.Analysis.Module.SampleTestResults;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application;

namespace HLab.Erp.Lims.Analysis.Module.SampleTests
{
    /// <summary>
    /// Logique d'interaction pour SampleTestView.xaml
    /// </summary>
    public partial class SampleTestResultView : UserControl, IView<SampleTestResultViewModel>, IViewClassDocument
    {
        public SampleTestResultView()
        {
            InitializeComponent();
        }
    }
}
