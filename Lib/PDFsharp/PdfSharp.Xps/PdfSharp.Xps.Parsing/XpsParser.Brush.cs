﻿using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a Brush element.
    /// </summary>
    Brush ParseBrush()
    {
      Brush brush = null;
      switch (this.reader.Name)
      {
        case "ImageBrush":
          brush = ParseImageBrush();
          break;

        case "SolidColorBrush":
          brush = ParseSolidColorBrush();
          break;

        case "LinearGradientBrush":
          brush = ParseLinearGradientBrush();
          break;

        case "RadialGradientBrush":
          brush = ParseRadialGradientBrush();
          break;

        case "VisualBrush":
          brush = ParseVisualBrush();
          break;

        default:
          Debugger.Break();
          break;
      }
      return brush;
    }

    /// <summary>
    /// Parses a Brush attribute.
    /// </summary>
    Brush ParseBrush(string value)
    {
      Brush brush = TryParseStaticResource<Brush>(value);
      if (brush != null)
        return brush;
      return Brush.Parse(value);
    }
  }
}