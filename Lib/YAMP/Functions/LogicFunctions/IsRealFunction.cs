namespace YAMP
{
	[Description("IsRealFunctionDescription")]
	[Kind(PopularKinds.Logic)]
    [Link("IsRealFunctionLink")]
    internal sealed class IsRealFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return new ScalarValue(value.IsReal);
        }
    }
}
