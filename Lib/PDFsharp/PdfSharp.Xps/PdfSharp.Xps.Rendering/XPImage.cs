using PdfSharp.Drawing;

using BitmapSource = System.Windows.Media.Imaging.BitmapSource;

namespace PdfSharp.Xps.Rendering
{
    internal class XPImage : XPObject
  {
    public XPImage(BitmapSource bitmapSource)
    {
      this.xImage = XImage.FromBitmapSource(bitmapSource);
    }

    public XImage XImage
    {
      get { return this.xImage; }
    }
    XImage xImage;
  }
}
