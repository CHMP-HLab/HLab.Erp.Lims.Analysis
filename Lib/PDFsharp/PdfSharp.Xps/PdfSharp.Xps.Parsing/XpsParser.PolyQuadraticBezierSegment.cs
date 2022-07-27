using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a PolyQuadraticBezierSegment element.
    /// </summary>
    PolyQuadraticBezierSegment ParsePolyQuadraticBezierSegment()
    {
      Debug.Assert(this.reader.Name == "PolyQuadraticBezierSegment");
      PolyQuadraticBezierSegment seg = new PolyQuadraticBezierSegment();
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