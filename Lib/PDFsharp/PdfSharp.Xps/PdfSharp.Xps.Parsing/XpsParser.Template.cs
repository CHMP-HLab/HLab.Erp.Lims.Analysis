using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a element.
    /// </summary>
    Canvas ParseObject()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      Canvas canvas = new Canvas();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Name":
            //canvas.Name = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
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
            case "Canvas.Resources":
              //MoveToNextElement();
              //canvas.Resources = ParseResourceDictionary();
              break;

            default:
              Debugger.Break();
              break;
          }
        }
      }
      MoveToNextElement();
      return canvas;
    }
  }
}