namespace YAMP
{
    [Description("ArcsecFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArcsecFunctionLink")]
    internal sealed class ArcsecFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arcsec();
        }
    }
}
