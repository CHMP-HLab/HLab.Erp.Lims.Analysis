namespace YAMP
{
    [Description("ArccosFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArccosFunctionLink")]
    internal sealed class ArccosFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arccos();
        }
    }
}
