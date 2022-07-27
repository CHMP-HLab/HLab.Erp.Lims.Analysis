namespace YAMP
{
    [Description("TanhFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("TanhFunctionLink")]
    internal sealed class TanhFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            var a = value.Exp();
            var b = (-value).Exp();
            return (a - b) / (a + b);
        }
    }
}
