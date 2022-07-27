namespace YAMP
{
	[Description("IsIntFunctionDescription")]
	[Kind(PopularKinds.Logic)]
    [Link("IsIntFunctionLink")]
    internal sealed class IsIntFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return new ScalarValue(value.IsInt);
        }
    }
}
