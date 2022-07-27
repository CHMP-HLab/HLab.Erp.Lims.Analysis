namespace YAMP
{
    using YAMP.Numerics;

    [Description("DigammaFunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class DigammaFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return new ScalarValue(Gamma.Psi(value.Re));
        }
    }
}
