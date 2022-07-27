using System;
using HLab.Erp.Conformity.Annotations;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities
{
    using H = HD<SampleForm>;
    public class SampleForm : Entity, IFormTarget, IListableModel
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

        readonly IForeign<FormClass> _formClass = H.Foreign<FormClass>();

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

        readonly IForeign<Sample> _sample = H.Foreign<Sample>();


        public ConformityState ConformityId
        {
            get =>_conformityId.Get(); 
            set =>_conformityId.Set(value); 
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        readonly IProperty<ConformityState> _conformityId = H.Property<ConformityState>();


        public string SpecificationValues
        {
            get => _specificationValues.Get(); 
            set => _specificationValues.Set(value);
        }

        readonly IProperty<string> _specificationValues = H.Property<string>();

        public string ResultValues
        {
            get => _resultValues.Get(); 
            set => _resultValues.Set(value);
        }

        readonly IProperty<string> _resultValues = H.Property<string>();

        public bool MandatoryDone
        {
            get => _mandatoryDone.Get(); 
            set => _mandatoryDone.Set(value);
        }

        readonly IProperty<bool> _mandatoryDone = H.Property<bool>();

        public bool SpecificationDone
        {
            get => _specificationDone.Get(); 
            set => _specificationDone.Set(value);
        }

        readonly IProperty<bool> _specificationDone = H.Property<bool>();

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

        public string Caption => FormClass?.Name;
    }
}
