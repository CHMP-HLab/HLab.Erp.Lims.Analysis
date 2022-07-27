namespace YAMP
{
    [Description("ArccotFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArccotFunctionLink")]
    internal sealed class ArccotFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arccot();
        }
    }
}
