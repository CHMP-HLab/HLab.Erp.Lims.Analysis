using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a SpotLocation element.
    /// </summary>
    SpotLocation ParseSpotLocation()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      SpotLocation spotLocation = new SpotLocation();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "StartX":
            spotLocation.StartX = double.Parse(this.reader.Value);
            break;

          case "StartY":
            spotLocation.StartY = double.Parse(this.reader.Value);
            break;

          case "PageURI":
            spotLocation.PageURI = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return spotLocation;
    }
  }
}