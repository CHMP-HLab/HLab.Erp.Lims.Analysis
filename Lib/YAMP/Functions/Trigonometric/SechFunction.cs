namespace YAMP
{
    [Description("SechFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("SechFunctionLink")]
    internal sealed class SechFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return 2.0 / (value.Exp() + (-value).Exp());
        }
    }
}
