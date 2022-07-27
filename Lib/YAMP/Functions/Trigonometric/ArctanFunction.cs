namespace YAMP
{
    [Description("ArctanFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArctanFunctionLink")]
    internal sealed class ArctanFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arctan();
        }
    }
}
