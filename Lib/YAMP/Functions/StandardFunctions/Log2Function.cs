﻿namespace YAMP
{
    [Description("Log2FunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class Log2Function : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Log(2.0);
        }
    }
}
