﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Describes a single structural block. These structural blocks are grouped together in a list.
    /// </summary>
    internal class ListItemStructure : XpsElement
  {
    /// <summary>
    /// The named element that represents the marker for this list items, such as a bullet,
    /// number, or image.
    /// </summary>
    public string Marker { get; set; }
  }
}