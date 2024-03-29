namespace YAMP
{
	[Description("FactorialFunctionDescription")]
	[Kind(PopularKinds.Function)]
    internal sealed class FactorialFunction : StandardFunction
	{
		protected override ScalarValue GetValue(ScalarValue value)
		{
			return value.Factorial();
		}
	}
}

