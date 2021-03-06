﻿namespace YAMP.Exceptions
{
    using System;

    /// <summary>
    /// The range invalid exception.
    /// </summary>
    public class YAMPRangeInvalidException : YAMPRuntimeException
    {
        internal YAMPRangeInvalidException(String msg)
            : base("The given range value is invalid, since {0}.", msg)
        {
        }
    }
}
