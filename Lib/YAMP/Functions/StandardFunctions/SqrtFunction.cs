namespace YAMP
{
	[Description("SqrtFunctionDescription")]
	[Kind(PopularKinds.Function)]
    internal sealed class SqrtFunction : StandardFunction
	{
		protected override ScalarValue GetValue(ScalarValue value)
		{
			return value.Sqrt();
		}
	}
}

