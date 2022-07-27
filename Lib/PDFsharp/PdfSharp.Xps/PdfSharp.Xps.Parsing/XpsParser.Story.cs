using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a Story element.
    /// </summary>
    Story ParseStory()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      Story story = new Story();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "StoryName":
            story.StoryName = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return story;
    }
  }
}