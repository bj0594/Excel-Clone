using System;
using System.Collections.Generic;
using System.Linq;
using ExcelClone.Data;

namespace ExcelClone.Models;

// Represents the complete spreadsheet.
//
// A Table owns its headers and data rows, while individual
// cells are accessed through the ICell abstraction. This
// allows the table to work with Cell<T> regardless of which
// concrete type is stored in a cell.
internal sealed class Table
{
    // The spreadsheet's data rows.
    //
    // Row itself manages the cells inside each row.
    private readonly List<Row> rows;

    // Headers are kept separately from data rows because
    // headers are always text and have different behaviour
    // from normal data cells.
    private readonly List<string> headers;

    // Number of rows containing editable data.
    public int DataRowCount
    {
        get;
    }

    // Number of columns in the table.
    public int ColumnCount
    {
        get;
    }

    // Display row count includes the header row.
    //
    // The editor uses this when moving the active cell
    // through the rendered table.
    public int TotalDisplayRows =>
        DataRowCount + 1;

    // Provides read-only access to the table's rows.
    //
    // IReadOnlyList prevents callers from replacing,
    // adding or removing rows directly.
    public IReadOnlyList<Row> Rows =>
        rows;

    // Provides read-only access to the table's headers.
    public IReadOnlyList<string> Headers =>
        headers;

    public Table(
        int dataRowCount,
        int columnCount)
    {
        // Keep the table small enough to remain practical
        // to display and navigate in a terminal.
        if (dataRowCount < 1 ||
            dataRowCount > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataRowCount));
        }

        if (columnCount < 1 ||
            columnCount > 8)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columnCount));
        }

        DataRowCount =
            dataRowCount;

        ColumnCount =
            columnCount;

        // Pre-size the list because the number of columns
        // is known when the table is created.
        headers =
            new List<string>(
                columnCount);

        for (int column = 0;
             column < columnCount;
             column++)
        {
            headers.Add(
                string.Empty);
        }

        // Pre-size the row collection for the same reason.
        rows =
            new List<Row>(
                dataRowCount);

        for (int row = 0;
             row < dataRowCount;
             row++)
        {
            rows.Add(
                new Row(
                    columnCount));
        }
    }

    // Returns a cell at a validated data position.
    //
    // ICell is returned instead of Cell<T> so callers do
    // not need to know the cell's concrete generic type.
    public ICell GetDataCell(
        int dataRow,
        int column)
    {
        ValidateDataPosition(
            dataRow,
            column);

        return rows[dataRow]
            .GetCell(
                column);
    }

    // Replaces a cell at a validated data position.
    //
    // The ICell parameter allows any supported Cell<T>
    // implementation to be stored in the table.
    public void SetDataCell(
        int dataRow,
        int column,
        ICell cell)
    {
        ValidateDataPosition(
            dataRow,
            column);

        rows[dataRow]
            .SetCell(
                column,
                cell);
    }

    // Returns the text stored in a header.
    public string GetHeader(
        int column)
    {
        ValidateColumn(
            column);

        return headers[column];
    }

    // Updates a header.
    //
    // Trimming here keeps accidental whitespace out of the
    // header value while leaving normal cell input untouched.
    public void SetHeader(
        int column,
        string value)
    {
        ValidateColumn(
            column);

        headers[column] =
            value.Trim();
    }

    // Builds a type profile for one column.
    //
    // Only the data rows are analysed; the header is not
    // part of the column's detected data type.
    public TypeProfile AnalyzeColumn(
        int column)
    {
        ValidateColumn(
            column);

        IEnumerable<ICell> cells =
            rows.Select(
                row =>
                    row.GetCell(
                        column));

        return TypeProfile.FromCells(
            cells);
    }

    // Validates a column index shared by headers and data.
    private void ValidateColumn(
        int column)
    {
        if (column < 0 ||
            column >= ColumnCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }
    }

    // Validates both dimensions of a data-cell position.
    private void ValidateDataPosition(
        int dataRow,
        int column)
    {
        ValidateColumn(
            column);

        if (dataRow < 0 ||
            dataRow >= DataRowCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataRow));
        }
    }
}