using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a NamedElement element.
    /// </summary>
    NamedElement ParseNamedElement()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      NamedElement namedElement = new NamedElement();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "NameReference":
            namedElement.NameReference = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return namedElement;
    }
  }
}