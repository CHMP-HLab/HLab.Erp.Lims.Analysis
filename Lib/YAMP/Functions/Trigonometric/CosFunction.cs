namespace YAMP
{
    [Description("CosFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("CosFunctionLink")]
    internal sealed class CosFunction : StandardFunction
	{
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Cos();
        }
	}
}

