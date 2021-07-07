#if false
namespace PdfSharp.Pdf.Advanced
{
  /// <summary>
  /// Contains all used Shading objects of a document.
  /// </summary>
  internal sealed class PdfShadingTable : PdfResourceTable
  {
    /// <summary>
    /// Initializes a new instance of this class, which is a singleton for each document.
    /// </summary>
    public PdfShadingTable(PdfDocument document) : base(document)
    {
    }

    /// <summary>
    /// Gets a PdfExtGState with the keys 'CA' and 'ca' set to the specified alpha value.
    /// </summary>
    public PdfExtGState GetExtGState(double alpha)
    {
      string key = MakeKey(alpha);
      PdfExtGState extGState = this.alphaValues[key] as PdfExtGState;
      if (extGState == null)
      {
        extGState = new PdfExtGState(this.document);
        extGState.Elements[PdfExtGState.Keys.CA] = new PdfReal(alpha);
        extGState.Elements[PdfExtGState.Keys.ca] = new PdfReal(alpha);

        this.alphaValues[key] = extGState;
      }
      return extGState;
    }

    /// <summary>
    /// Gets a PdfExtGState with the key 'CA' set to the specified alpha value.
    /// </summary>
    public PdfExtGState GetExtGStateStroke(double alpha)
    {
      string key = MakeKey(alpha);
      PdfExtGState extGState = this.strokeAlphaValues[key] as PdfExtGState;
      if (extGState == null)
      {
        extGState = new PdfExtGState(this.document);
        extGState.Elements[PdfExtGState.Keys.CA] = new PdfReal(alpha);

        this.strokeAlphaValues[key] = extGState;
      }
      return extGState;
    }

    /// <summary>
    /// Gets a PdfExtGState with the key 'ca' set to the specified alpha value.
    /// </summary>
    public PdfExtGState GetExtGStateNonStroke(double alpha)
    {
      string key = MakeKey(alpha);
      PdfExtGState extGState = this.nonStrokeAlphaValues[key] as PdfExtGState;
      if (extGState == null)
      {
        extGState = new PdfExtGState(this.document);
        extGState.Elements[PdfExtGState.Keys.ca] = new PdfReal(alpha);

        this.nonStrokeAlphaValues[key] = extGState;
      }
      return extGState;
    }

    /// <summary>
    /// Gets a PdfExtGState with the key 'ca' set to the specified alpha value.
    /// </summary>
    public PdfExtGState GetExtGState(XColor strokeColor, XColor nonStrokeColor)
    {
      if (strokeColor.IsEmpty)
      {
      }
      else if (nonStrokeColor.IsEmpty)
      {
      }
      else
      {
      }

      return null;
      //string key = MakeKey(alpha);
      //PdfExtGState extGState = this.nonStrokeAlphaValues[key] as PdfExtGState;
      //if (extGState == null)
      //{
      //  extGState = new PdfExtGState(this.document);
      //  extGState.Elements[PdfExtGState.Keys.ca] = new PdfReal(alpha);
      //
      //  this.nonStrokeAlphaValues[key] = extGState;
      //}
      //return extGState;
    }

    string MakeKey(double alpha)
    {
      return ((int)(1000 * alpha)).ToString();
    }

    /// <summary>
    /// Maps from alpha values (range "0" to "1000") to PdfExtGState objects.
    /// </summary>
    Hashtable alphaValues = new Hashtable();
    Hashtable strokeAlphaValues = new Hashtable();
    Hashtable nonStrokeAlphaValues = new Hashtable();
  }
}
#endif