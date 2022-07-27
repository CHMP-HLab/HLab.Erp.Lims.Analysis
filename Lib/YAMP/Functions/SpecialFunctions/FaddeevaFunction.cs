namespace YAMP
{
    using YAMP.Numerics;

    [Description("FaddeevaFunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class FaddeevaFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return ErrorFunction.Faddeeva(value);
        }
    }
}
