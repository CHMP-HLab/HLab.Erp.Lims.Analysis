using HLab.Erp.Conformity.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Lims.Analysis.Module.FormClasses;

internal class DummyTarget : NotifierBase, IFormTarget
{
    public DummyTarget() => H<DummyTarget>.Initialize(this);

    public string Result
    {
        get => _result.Get();
        set => _result.Set(value);
    }

    readonly IProperty<string> _result = H<DummyTarget>.Property<string>();

    public ConformityState ConformityId
    {
        get =>_conformityId.Get(); 
        set =>_conformityId.Set(value); 
    }

    readonly IProperty<ConformityState> _conformityId = H<DummyTarget>.Property<ConformityState>();

    public byte[] Code => null;

    public string SpecificationValues
    {
        get => _specificationValues.Get(); 
        set => _specificationValues.Set(value);
    }

    readonly IProperty<string> _specificationValues = H<DummyTarget>.Property<string>();

    public bool SpecificationDone
    {
        get => _specificationDone.Get(); 
        set => _specificationDone.Set(value);
    }

    readonly IProperty<bool> _specificationDone = H<DummyTarget>.Property<bool>();

    public string ResultValues
    {
        get => _resultValues.Get(); 
        set => _resultValues.Set(value);
    }

    readonly IProperty<string> _resultValues = H<DummyTarget>.Property<string>();

    public bool MandatoryDone
    {
        get => _mandatoryDone.Get(); 
        set => _mandatoryDone.Set(value);
    }

    readonly IProperty<bool> _mandatoryDone = H<DummyTarget>.Property<bool>();


    public string DefaultTestName => "Dummy";
    public string TestName
    {
        get => _testName.Get();
        set => _testName.Set(value);
    }

    readonly IProperty<string> _testName = H<DummyTarget>.Property<string>();

    public string Description
    {
        get => _description.Get();
        set => _description.Set(value);
    }

    readonly IProperty<string> _description = H<DummyTarget>.Property<string>();

    public string Specification
    {
        get => _specification.Get();
        set => _specification.Set(value);
    }

    readonly IProperty<string> _specification = H<DummyTarget>.Property<string>();

    public string Conformity
    {
        get => _conformity.Get();
        set => _conformity.Set(value);
    }

    readonly IProperty<string> _conformity = H<DummyTarget>.Property<string>();

    IFormClass IFormTarget.FormClass { get => null; set => throw new System.InvalidOperationException($"Setting {nameof(IFormTarget.FormClass)} not allowed"); }

    public string Name
    {
        get => _name.Get();
        set => _name.Set(value);
    }

    readonly IProperty<string> _name = H<DummyTarget>.Property<string>(c => c.Default("Dummy"));
}