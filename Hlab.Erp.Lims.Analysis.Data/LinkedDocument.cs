using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    public class LinkedDocument : Entity
    {
        public LinkedDocument() => HD<LinkedDocument>.Initialize(this);
        

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }

        private readonly IProperty<string> _name = HD<LinkedDocument>.Property<string>(c => c.Default(""));
        public int? SampleTestResultId
        {
            get => _sampleTestResult.Id.Get();
            set => _sampleTestResult.Id.Set(value);
        }

        [Ignore] public SampleTestResult SampleTestResult
        {
            get => _sampleTestResult.Get();
            set => _sampleTestResult.Set(value);
        }
        private readonly IForeign<SampleTestResult> _sampleTestResult = HD<LinkedDocument>.Foreign<SampleTestResult>();

        public byte[] File
        {
            get => _file.Get();
            set => _file.Set(value);
        }
        private readonly IProperty<byte[]> _file = HD<LinkedDocument>.Property<byte[]>();
    }
}
