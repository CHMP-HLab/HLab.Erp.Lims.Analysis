namespace YAMP
{
    [Description("CoshFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("CoshFunctionLink")]
    internal sealed class CoshFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return (value.Exp() + (-value).Exp()) / 2.0;
        }
    }
}
