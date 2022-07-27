namespace YAMP
{
    [Description("ArcschFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArcschFunctionLink")]
    internal sealed class ArcschFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return (1.0 / z + (1.0 / z.Square() + 1.0).Sqrt()).Ln();
        }
    }
}
