using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal abstract class XPTilingBrush : XPBrush
  {
    protected XPTilingBrush(Brush brush)
      : base(brush)
    {
    }
  }
}
