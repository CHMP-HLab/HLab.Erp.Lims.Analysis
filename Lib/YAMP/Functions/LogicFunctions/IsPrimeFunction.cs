namespace YAMP
{
	[Description("IsPrimeFunctionDescription")]
	[Kind(PopularKinds.Logic)]
    [Link("IsPrimeFunctionLink")]
    internal sealed class IsPrimeFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return new ScalarValue(value.IsPrime);
        }
    }
}
