namespace YAMP
{
    [Description("ArccscFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArccscFunctionLink")]
    internal sealed class ArccscFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arccsc();
        }
    }
}
