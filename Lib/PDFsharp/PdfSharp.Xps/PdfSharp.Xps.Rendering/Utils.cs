﻿using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    /// <summary>
    /// Some temporary stuff.
    /// </summary>
    internal static class Utils
  {
    public static Color AlphaToGray(Color color)
    {
      return Color.FromArgb(255, color.A, color.A, color.A);
    }
  }
}