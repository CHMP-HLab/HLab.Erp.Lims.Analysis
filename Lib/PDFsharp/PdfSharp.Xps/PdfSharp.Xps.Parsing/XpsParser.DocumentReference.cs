using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a DocumentReference element.
    /// </summary>
    DocumentReference ParseDocumentReference()
    {
      Debug.Assert(this.reader.Name == "DocumentReference");
      bool isEmptyElement = this.reader.IsEmptyElement;
      DocumentReference documentReference = new DocumentReference();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Source":
            documentReference.Source = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return documentReference;
    }
  }
}