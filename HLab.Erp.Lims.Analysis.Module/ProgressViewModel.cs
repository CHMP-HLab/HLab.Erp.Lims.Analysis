using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module
{
    class ProgressViewModel : ViewModel<ProgressViewModel>
    {
        public double Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private IProperty<double> _value = H.Property<double>();
    }
}