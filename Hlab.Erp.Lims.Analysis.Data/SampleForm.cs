using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
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

        [Ignore] IFormClass IFormTarget.FormClass 
        {
            get => FormClass;
            set => FormClass = (FormClass)value;
        }


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


        public ConformityState ConformityId
        {
            get =>_conformityId.Get(); 
            set =>_conformityId.Set(value); 
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>();


        public string SpecificationValues
        {
            get => _specificationValues.Get(); 
            set => _specificationValues.Set(value);
        }
        private readonly IProperty<string> _specificationValues = H.Property<string>();

        public string ResultValues
        {
            get => _resultValues.Get(); 
            set => _resultValues.Set(value);
        }
        private readonly IProperty<string> _resultValues = H.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }
        private readonly IProperty<bool> _mandatoryDone = H.Property<bool>();

        public bool SpecificationDone
        {
            get => _specificationDone.Get(); 
            set => _specificationDone.Set(value);
        }
        private readonly IProperty<bool> _specificationDone = H.Property<bool>();

        byte[] IFormTarget.Code => FormClass.Code;
        string IFormTarget.TestName { get; set; }
        string IFormTarget.Description { get; set; }
        string IFormTarget.Specification { get; set; }
        string IFormTarget.Conformity { get; set; }
        string IFormTarget.Result { get; set; }

        string IFormTarget.DefaultTestName => FormClass.Name;
        string IFormTarget.Name
        {
            get => FormClass.Name;
            set => FormClass.Name = value;
        }
    }
}
