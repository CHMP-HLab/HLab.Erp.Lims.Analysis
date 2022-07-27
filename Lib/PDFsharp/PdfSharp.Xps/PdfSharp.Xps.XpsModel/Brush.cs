﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Base class of all five brush types
    /// </summary>
    internal class Brush : XpsElement
  {
    internal static Brush Parse(string value)
    {
      SolidColorBrush brush = new SolidColorBrush();
      brush.Color = Color.Parse(value);
      return brush;
    }
  }
}