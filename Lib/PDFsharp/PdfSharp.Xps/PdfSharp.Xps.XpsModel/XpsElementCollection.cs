using System.Collections.Generic;

namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Represents a collection of XpsElement objecs.
    /// </summary>
    internal class XpsElementCollection : List<XpsElement>
  {
    // Currently just a placeholder of a generic list.
  }

  /// <summary>
  /// Represents a collection of XpsElement objecs.
  /// </summary>
  internal class XpsElementCollection<T> : List<T> where T : XpsElement
  {
    // Currently just a placeholder of a generic list.
  }
}