namespace YAMP
{
    [Description("SinFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("SinFunctionLink")]
    internal sealed class SinFunction : StandardFunction
	{
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Sin();
        }
	}
}

