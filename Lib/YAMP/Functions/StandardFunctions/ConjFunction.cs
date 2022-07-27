namespace YAMP
{
    [Description("ConjFunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class ConjFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Conjugate();
        }
    }
}
