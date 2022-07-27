namespace YAMP
{
	[Description("IsComplexFunctionDescription")]
	[Kind(PopularKinds.Logic)]
    [Link("IsComplexFunctionLink")]
    internal sealed class IsComplexFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return new ScalarValue(value.IsComplex);
        }
    }
}
