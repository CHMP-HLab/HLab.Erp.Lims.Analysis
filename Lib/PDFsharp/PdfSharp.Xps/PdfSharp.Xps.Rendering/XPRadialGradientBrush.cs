using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal class XPRadialGradientBrush : XPGradientBrush
  {
    protected XPRadialGradientBrush(RadialGradientBrush brush)
      : base(brush)
    {
    }
  }
}
