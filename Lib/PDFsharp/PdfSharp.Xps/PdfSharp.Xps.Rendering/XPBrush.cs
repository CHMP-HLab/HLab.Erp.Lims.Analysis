using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal class XPBrush : XPObject
  {
    protected XPBrush(Brush brush)
    {
      this.brush = brush;
    }

    Brush Brush
    {
      get { return this.brush; }
    }
    Brush brush;
  }
}
