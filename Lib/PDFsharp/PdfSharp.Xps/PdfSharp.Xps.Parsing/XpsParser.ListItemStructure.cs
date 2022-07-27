using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a ListItemStructure element.
    /// </summary>
    ListItemStructure ParseListItemStructure()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      ListItemStructure listItemStructure = new ListItemStructure();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Marker":
            listItemStructure.Marker = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return listItemStructure;
    }
  }
}