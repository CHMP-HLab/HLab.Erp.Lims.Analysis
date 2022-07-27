using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Rendering
{
    internal class XPLinearGradientBrush : XPGradientBrush
  {
    protected XPLinearGradientBrush(LinearGradientBrush brush)
      : base(brush)
    {
    }
  }
}
