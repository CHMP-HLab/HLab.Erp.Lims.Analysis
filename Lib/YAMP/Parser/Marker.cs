﻿namespace YAMP
{
    using System;

    /// <summary>
    /// This is an enumeration of parse markers.
    /// </summary>
    [Flags]
    internal enum Marker
    {
        None = 0x0,
        Breakable = 0x1
    }
}
