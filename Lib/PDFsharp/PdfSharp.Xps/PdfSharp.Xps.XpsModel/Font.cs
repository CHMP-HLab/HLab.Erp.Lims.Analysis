﻿using PdfSharp.Pdf.Advanced;

namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Represents a font or font subset.
    /// </summary>
    internal class Font
  {
    public Font(string name, byte[] fontData)
    {
      Name = name;
      this.fontData = fontData;
    }

    /// <summary>
    /// Gets or sets the name of the font.
    /// </summary>
    public string Name { get; set; }

    public PdfFont PdfFont
    {
      get { return this.pdfFont; }
      set { this.pdfFont = value; }
    }
    PdfFont pdfFont;

    public byte[] FontData
    {
      get { return this.fontData; }
    }
    byte[] fontData;
  }
}