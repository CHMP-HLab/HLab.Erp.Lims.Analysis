using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a LinkTarget element.
    /// </summary>
    LinkTarget ParseLinkTarget()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      LinkTarget linkTarget = new LinkTarget();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Name":
            linkTarget.Name = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return linkTarget;
    }
  }
}