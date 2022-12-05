using System;
using HLab.Base.Extensions;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;

namespace HLab.Erp.Lims.Analysis.Data.Entities;

using H = HD<SampleMovement>;

public class SampleMovement : Entity
{
    public SampleMovement() => H.Initialize(this);
    /// <summary>
    /// Sample
    /// </summary>
    public int? SampleId
    {
        get => _sample.Id.Get();
        set => _sample.Id.Set(value);
    }
    [Ignore] public virtual Sample Sample
    {
        get => _sample.Get();
        set => SampleId = value.Id;
    }
    readonly IForeign<Sample> _sample = H.Foreign<Sample>();

    /// <summary>
    /// SampleTestResult
    /// </summary>
    public int? SampleTestResultId
    {
        get => _sampleTestResult.Id.Get();
        set => _sampleTestResult.Id.Set(value);
    }
    [Ignore] public virtual SampleTestResult SampleTestResult
    {
        get => _sampleTestResult.Get();
        set => SampleTestResultId = value?.Id;
    }
    readonly IForeign<SampleTestResult> _sampleTestResult = H.Foreign<SampleTestResult>();

    /// <summary>
    /// Motivation
    /// </summary>
    public int? MotivationId
    {
        get => _motivation.Id.Get();
        set => _motivation.Id.Set(value);
    }
    [Ignore] public virtual SampleMovementMotivation Motivation
    {
        get => _motivation.Get();
        set => MotivationId = value.Id;
    }
    readonly IForeign<SampleMovementMotivation> _motivation = H.Foreign<SampleMovementMotivation>();

    /// <summary>
    /// Quantity
    /// </summary>
    public double Quantity
    {
        get => _quantity.Get();
        set => _quantity.Set(value);
    }
    readonly IProperty<double> _quantity = H.Property<double>();

    public static SampleMovement DesignModel => new()
    {
        Motivation = new() { Name = "{use}" },
        Quantity = 20,
    };

    public DateTime Date
    {
        get => _date.Get().ToUniversalTime();
        set => _date.Set(value.ToUniversalTime());
    }
    readonly IProperty<DateTime> _date = H.Property<DateTime>();

}