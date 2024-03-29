namespace YAMP
{
    using System;

    [Description("CeilFunctionDescription")]
	[Kind(PopularKinds.Function)]
    internal sealed class CeilFunction : StandardFunction
	{
		protected override ScalarValue GetValue(ScalarValue value)
		{
			var re = Math.Ceiling(value.Re);
			var im = Math.Ceiling(value.Im);
			return new ScalarValue(re, im);
		}	
	}
}

