using System.Diagnostics;
using PdfSharp.Xps.XpsModel;

namespace PdfSharp.Xps.Parsing
{
    internal partial class XpsParser
  {
    /// <summary>
    /// Parses a StoryFragment element.
    /// </summary>
    StoryFragment ParseStoryFragment()
    {
      Debug.Assert(this.reader.Name == "");
      bool isEmptyElement = this.reader.IsEmptyElement;
      StoryFragment storyFragment = new StoryFragment();
      while (MoveToNextAttribute())
      {
        switch (this.reader.Name)
        {
          case "StoryName":
            storyFragment.StoryName = this.reader.Value;
            break;

          case "FragmentName":
            storyFragment.FragmentName = this.reader.Value;
            break;

          case "FragmentType":
            storyFragment.FragmentType = this.reader.Value;
            break;
          
          default:
            UnexpectedAttribute(this.reader.Name);
            break;
        }
      }
      MoveToNextElement();
      return storyFragment;
    }
  }
}