﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Contains a string value that identifies the current element as a named,
    /// addressable point in the document for the purpose of hyperlinking.
    /// </summary>
    internal class LinkTarget : XpsElement
  {
    /// <summary>
    /// Specifies an addressable point on the page.
    /// </summary>
    public string Name { get; set; }
  }
}