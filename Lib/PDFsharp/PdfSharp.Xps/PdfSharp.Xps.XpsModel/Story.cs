﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Defines a single story and where each of its story fragments appear in the XPS Document.
    /// </summary>
    internal class Story : XpsElement
  {
    /// <summary>
    /// The name used by story fragments to identify they belong to this story.
    /// </summary>
    public string StoryName { get; set; }
  }
}