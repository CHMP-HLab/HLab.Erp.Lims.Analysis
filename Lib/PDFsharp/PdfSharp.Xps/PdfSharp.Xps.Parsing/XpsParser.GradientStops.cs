using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a sequence of GradientStop element.
    /// </summary>
    GradientStopCollection ParseGradientStops()
    {
      GradientStopCollection gradientStops = new GradientStopCollection();
      if (!this.reader.IsEmptyElement)
      {
        MoveToNextElement();
        while (this.reader.IsStartElement())
        {
          GradientStop gs = ParseGradientStop();
          gradientStops.Add(gs);
        }
        MoveToNextElement();
      }
      return gradientStops;
    }
  }
}