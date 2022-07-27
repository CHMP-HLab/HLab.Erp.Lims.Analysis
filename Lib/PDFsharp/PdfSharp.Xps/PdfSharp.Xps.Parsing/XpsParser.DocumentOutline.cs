using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a DocumentOutline element.
    /// </summary>
    DocumentOutline ParseDocumentOutline()
    {
      Debug.Assert(this.reader.Name == "DocumentOutline");
      bool isEmptyElement = this.reader.IsEmptyElement;
      DocumentOutline documentOutline = new DocumentOutline();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "xml:lang":
            documentOutline.lang = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return documentOutline;
    }
  }
}