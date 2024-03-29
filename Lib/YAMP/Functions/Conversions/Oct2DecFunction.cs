﻿namespace YAMP
{
    using System;
    using System.Collections.Generic;
    using YAMP.Exceptions;

    [Description("Oct2DecFunctionDescription")]
    [Kind(PopularKinds.Conversion)]
    internal sealed class Oct2DecFunction : ArgumentFunction
    {
        [Description("Oct2DecFunctionDescriptionForString")]
        [Example("oct2dec(\"1627\")", "Oct2DecFunctionExampleForString1")]
        public ScalarValue Function(StringValue octstr)
        {
            var sum = 0;
            var hex = new Stack<Int32>();
            var weight = 1;

            for (var i = 1; i <= octstr.Length; i++)
            {
                var chr = octstr[i];

                if (!ParseEngine.IsWhiteSpace(chr) && !ParseEngine.IsNewLine(chr))
                {
                    if (chr >= '0' && chr <= '7')
                    {
                        hex.Push((Int32)(chr - '0'));
                    }
                    else
                    {
                        throw new YAMPRuntimeException("oct2dec can only interpret octal strings.");
                    }
                }
            }

            while (hex.Count != 0)
            {
                var el = hex.Pop();
                sum += weight * el;
                weight *= 8;
            }

            return new ScalarValue(sum);
        }
    }
}
