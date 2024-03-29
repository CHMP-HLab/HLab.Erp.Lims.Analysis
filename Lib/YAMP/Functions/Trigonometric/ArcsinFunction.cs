﻿namespace YAMP
{
    [Description("ArcsinFunctionDescription")]
    [Kind(PopularKinds.Trigonometric)]
    [Link("ArcsinFunctionLink")]
    internal sealed class ArcsinFunction : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue z)
        {
            return z.Arcsin();
        }
    }
}
