﻿namespace PdfSharp.Xps.XpsModel
{
    /// <summary>
    /// Contains the elements that occupy a single cell of a table.
    /// </summary>
    internal class TableCellStructure : XpsElement
  {
    /// <summary>
    /// Indicates the number of rows this cell spans, or merges into a single cell.
    /// </summary>
    public int RowSpan { get; set; }

    /// <summary>
    /// Indicates the number of columns this cell spans, or merges into a single cell.
    /// </summary>
    public int ColumnSpan { get; set; }
  }
}