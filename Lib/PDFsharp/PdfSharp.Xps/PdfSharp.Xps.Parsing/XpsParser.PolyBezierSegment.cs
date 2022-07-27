using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a PolyBezierSegment element.
    /// </summary>
    PolyBezierSegment ParsePolyBezierSegment()
    {
      Debug.Assert(this.reader.Name == "PolyBezierSegment");
      PolyBezierSegment seg = new PolyBezierSegment();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "IsStroked":
            seg.IsStroked = ParseBool(this.reader.Value);
            break;

          case "Points":
            seg.Points = Point.ParsePoints(this.reader.Value);
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveBeyondThisElement();
      return seg;
    }
  }
}