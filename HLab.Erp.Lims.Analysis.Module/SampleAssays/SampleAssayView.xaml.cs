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
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Lims.Analysis.Module.SampleAssays
{
    /// <summary>
    /// Logique d'interaction pour SampleAssayView.xaml
    /// </summary>
    public partial class SampleAssayView : UserControl, IView<SampleAssayViewModel>, IViewClassDocument
    {
        public SampleAssayView()
        {
            InitializeComponent();
        }
    }
}
