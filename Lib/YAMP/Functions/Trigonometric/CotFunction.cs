namespace YAMP
{
    [Description("CotFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("CotFunctionLink")]
    internal sealed class CotFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Cos() / value.Sin();
        }
    }
}
