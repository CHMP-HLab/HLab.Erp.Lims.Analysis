namespace YAMP
{
	[Description("ImagFunctionDescription")]
	[Kind(PopularKinds.Function)]
    internal sealed class ImagFunction : StandardFunction
	{
		protected override ScalarValue GetValue(ScalarValue value)
		{
			return new ScalarValue(value.Im);
		}
	}
}
