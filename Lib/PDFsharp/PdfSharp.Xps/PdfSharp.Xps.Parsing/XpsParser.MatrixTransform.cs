using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a MatrixTransform element.
    /// </summary>
    MatrixTransform ParseMatrixTransform()
    {
      AssertElement("MatrixTransform");
      MatrixTransform transform = new MatrixTransform();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Matrix":
            transform.Matrix = Matrix.Parse(this.reader.Value);
            break;

          case "x:Key":
            transform.Key = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveBeyondThisElement();
      return transform;
    }

    /// <summary>
    /// Parses a MatrixTransform attribute.
    /// </summary>
    MatrixTransform ParseMatrixTransform(string value)
    {
      MatrixTransform transform = TryParseStaticResource<MatrixTransform>(value);
      if (transform != null)
        return transform;

      return new MatrixTransform(Matrix.Parse(value));
    }
  }
}