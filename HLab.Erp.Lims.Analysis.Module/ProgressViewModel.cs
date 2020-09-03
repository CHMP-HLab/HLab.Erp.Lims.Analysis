using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    class ProgressViewModel : ViewModel
    {
        public ProgressViewModel() => H<ProgressViewModel>.Initialize(this);

        public double Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<double> _value = H<ProgressViewModel>.Property<double>();
    }
}