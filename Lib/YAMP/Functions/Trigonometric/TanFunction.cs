namespace YAMP
{
    [Description("TanFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("TanFunctionLink")]
    internal sealed class TanFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Sin() / value.Cos();
        }
    }
}
