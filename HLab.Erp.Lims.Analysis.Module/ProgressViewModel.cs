using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    public class ProgressViewModel : ViewModel
    {
        public ProgressViewModel() => H<ProgressViewModel>.Initialize(this);

        public double Value
        {
            get => _value.Get();
            set { if( _value.Set(value)) Percent = value * 100.0; }
        }

        private readonly IProperty<double> _value = H<ProgressViewModel>.Property<double>();
        public double Percent
        {
            get => _percent.Get();
            set
            {
                if (_percent.Set(value)) Value = value / 100.0;
            }
        }

        private readonly IProperty<double> _percent = H<ProgressViewModel>.Property<double>();
    }
}