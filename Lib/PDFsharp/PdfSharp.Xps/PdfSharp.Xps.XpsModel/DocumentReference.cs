namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Contains a reference to a FixedDocument part.
    /// </summary>
    internal class DocumentReference : XpsElement
  {
    /// <summary>
    /// Specifies the URI of the fixed document content.
    /// The specified URI MUST refer to a FixedDocument part within the XPS Document [M3.2].
    /// </summary>
    public string Source { get; set; }
  }
}