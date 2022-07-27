using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a FixedDocumentSequence element.
    /// </summary>
    FixedDocumentSequence ParseFixedDocumentSequence()
    {
      Debug.Assert(this.reader.Name == "FixedDocumentSequence");
      bool isEmptyElement = this.reader.IsEmptyElement;
      FixedDocumentSequence fdseq = new FixedDocumentSequence();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          default:
            //UnexpectedAttribute();
            break;
        }
      }
      if (!isEmptyElement)
      {
        MoveToNextElement();
        while (this.reader.IsStartElement())
        {
          switch (this.reader.Name)
          {
            case "DocumentReference":
              {
                PdfSharp.Xps.XpsModel.DocumentReference dref = ParseDocumentReference();
                //Debug.WriteLine("Path: " + (path.Name != null ? path.Name : ""));
                fdseq.DocumentReferences.Add(dref);
              }
              break;

            default:
              Debugger.Break();
              break;
          }
        }
      }
      MoveToNextElement();
      return fdseq;
    }
  }
}