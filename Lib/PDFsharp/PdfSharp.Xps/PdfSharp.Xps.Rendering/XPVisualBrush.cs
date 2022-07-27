using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal class XPVisualBrush : XPTilingBrush
  {
    public XPVisualBrush(ImageBrush brush)
      : base(brush)
    {
    }
  }
}
