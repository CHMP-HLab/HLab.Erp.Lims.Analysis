﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Fills a region with a linear gradient.
    /// </summary>
    internal class LinearGradientBrush : Brush
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearGradientBrush"/> class.
    /// </summary>
    public LinearGradientBrush()
    {
      Opacity = 1;
      Transform = new MatrixTransform();
    }

    /// <summary>
    /// Defines the uniform transparency of the brush fill. Values range from 0 (fully transparent)
    /// to 1 (fully opaque), inclusive. Values outside of this range are invalid.
    public double Opacity { get; set; }

    /// <summary>
    /// Specifies a name for a resource in a resource dictionary. x:Key MUST be present when the
    /// current element is defined in a resource dictionary. x:Key MUST NOT be specified outside of
    /// a resource dictionary [M6.4].
    /// </summary>
    // x:Key
    public string Key { get; set; }    

    /// <summary>
    /// Specifies the gamma function for color interpolation. The gamma adjustment should not be
    /// applied to the alpha component, if specified. Valid values are SRgbLinearInterpolation and
    /// ScRgbLinearInterpolatio n. 
    /// </summary>
    public ClrIntMode ColorInterpolationMode { get; set; }

    /// <summary>
    /// Describes how the brush should fill the content area outside of the primary, initial gradient area.
    /// Valid values are Pad, Reflect and Repeat.
    /// </summary>
    public SpreadMethod SpreadMethod { get; set; }

    /// <summary>
    /// Specifies that the start point and end point are defined in the effective coordinate space
    /// (includes the Transform attribute of the brush). 
    /// </summary>
    public MappingMode MappingMode { get; set; }

    /// <summary>
    /// Describes the matrix transformation applied to the coordinate space of the brush.
    /// The Transform property is concatenated with the current effective render transform to yield
    /// an effective render transform local to the brush. The viewport for the brush is transformed
    /// using that local effective render transform. 
    /// </summary>
    public MatrixTransform Transform { get; set; }

    /// <summary>
    /// Specifies the starting point of the linear gradient.
    /// </summary>
    public Point StartPoint { get; set; }

    /// <summary>
    /// Specifies the end point of the linear gradient. The linear gradient brush interpolates the colors
    /// from the start point to the end point, where the start point represents an offset of 0, and the
    /// EndPoint represents an offset of 1. The Offset attribute value specified in a GradientStop
    /// element relates to the 0 and 1 offsets defined by the start point and end point. 
    /// </summary>
    public Point EndPoint { get; set; }

    /// <summary>
    /// Holds a sequence of <GradientStop> elements.
    /// </summary>
    public GradientStopCollection GradientStops { get; set; }
  }
}