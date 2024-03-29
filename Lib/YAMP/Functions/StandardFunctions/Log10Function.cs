﻿namespace YAMP
{
    [Description("Log10FunctionDescription")]
    [Kind(PopularKinds.Function)]
    internal sealed class Log10Function : StandardFunction
    {
        protected override ScalarValue GetValue(ScalarValue value)
        {
            return value.Log10();
        }
    }
}
