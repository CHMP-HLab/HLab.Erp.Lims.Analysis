using HLab.Erp.Conformity.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses
{
    internal class DummyTarget : NotifierBase, IFormTarget
    {
        public DummyTarget() => H<DummyTarget>.Initialize(this);

        public string Result { get; set; }

        public ConformityState ConformityId
        {
            get =>_conformityId.Get(); 
            set =>_conformityId.Set(value); 
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        private readonly IProperty<ConformityState> _conformityId = H<DummyTarget>.Property<ConformityState>();

        public byte[] Code => null;

        public string SpecificationValues
        {
            get => _specificationValues.Get(); 
            set => _specificationValues.Set(value);
        }
        private readonly IProperty<string> _specificationValues = H<DummyTarget>.Property<string>();

        public bool SpecificationDone
        {
            get => _specificationDone.Get(); 
            set => _specificationDone.Set(value);
        }
        private readonly IProperty<bool> _specificationDone = H<DummyTarget>.Property<bool>();

        public string ResultValues
        {
            get => _resultValues.Get(); 
            set => _resultValues.Set(value);
        }
        private readonly IProperty<string> _resultValues = H<DummyTarget>.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H<DummyTarget>.Property<bool>();


        public string DefaultTestName => "Dummy";
        public string TestName { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Conformity { get; set; }
        IFormClass IFormTarget.FormClass { get => null; set => throw new System.NotImplementedException(); }
        string IFormTarget.Name { get => "Dummy"; set => throw new System.NotImplementedException(); }
    }
}
