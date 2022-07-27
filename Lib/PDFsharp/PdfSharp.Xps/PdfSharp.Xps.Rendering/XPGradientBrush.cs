using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal abstract class XPGradientBrush : XPBrush
  {
    protected XPGradientBrush(Brush brush)
      : base(brush)
    {
    }
  }
}
