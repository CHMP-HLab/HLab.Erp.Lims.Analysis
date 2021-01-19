using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Data;
using HLab.Erp.Forms.Annotations;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data
{
    using H = HD<SampleForm>;
    public class SampleForm : Entity, IFormTarget
    {
        public SampleForm() => H.Initialize(this);
        
        public int? FormClassId
        {
            get => _formClass.Id.Get();
            set => _formClass.Id.Set(value);
        }
        [Ignore]
        public FormClass FormClass
        {
            set => _formClass.Set(value);
            get => _formClass.Get();
        }
        private readonly IForeign<FormClass> _formClass = H.Foreign<FormClass>();

        public int? SampleId
        {
            get => _sample.Id.Get();
            set => _sample.Id.Set(value);
        }
        [Ignore]
        public Sample Sample
        {
            set => _sample.Set(value);
            get => _sample.Get();
        }
        private readonly IForeign<Sample> _sample = H.Foreign<Sample>();

        public FormState State
        {
            get =>_state.Get(); 
            set =>_state.Set(value); 
        }
        private readonly IProperty<FormState> _state = H.Property<FormState>();

        public string SpecValues
        {
            get => _specValues.Get(); 
            set => _specValues.Set(value);
        }
        private readonly IProperty<string> _specValues = H.Property<string>();

        public string Values
        {
            get => _values.Get(); 
            set => _values.Set(value);
        }
        private readonly IProperty<string> _values = H.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H.Property<bool>();

        public bool SpecificationsDone
        {
            get => _specificationsDone.Get(); 
            set => _specificationsDone.Set(value);
        }
        private readonly IProperty<bool> _specificationsDone = H.Property<bool>();
    }
}
