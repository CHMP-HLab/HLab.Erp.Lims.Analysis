using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a StoryFragmentReference element.
    /// </summary>
    StoryFragmentReference ParseStoryFragmentReference()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      StoryFragmentReference storyFragmentReference = new StoryFragmentReference();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "Page":
            storyFragmentReference.Page = int.Parse(this.reader.Value);
            break;

          case "FragmentName":
            storyFragmentReference.FragmentName = this.reader.Value;
            break;

          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return storyFragmentReference;
    }
  }
}