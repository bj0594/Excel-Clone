using System.Collections.Generic;
using System.Linq;

namespace ExcelClone.Models;

internal sealed class Table
{
    private readonly List<Row> rows;

    private readonly List<string> headers;

    public int DataRowCount { get; }

    public int ColumnCount { get; }

    public int TotalDisplayRows =>
        DataRowCount + 1;

    public IReadOnlyList<Row> Rows =>
        rows;

    public IReadOnlyList<string> Headers =>
        headers;

    public Table(
        int dataRowCount,
        int columnCount)
    {
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

    public ICell GetDataCell(
        int dataRow,
        int column)
    {
        ValidateDataPosition(
            dataRow,
            column);

        return rows[dataRow]
            .GetCell(column);
    }

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

    public string GetHeader(
        int column)
    {
        ValidateColumn(column);

        return headers[column];
    }

    public void SetHeader(
        int column,
        string value)
    {
        ValidateColumn(column);

        headers[column] =
            value.Trim();
    }

    public TypeProfile AnalyzeColumn(
        int column)
    {
        ValidateColumn(column);

        IEnumerable<ICell> cells =
            rows.Select(
                row =>
                    row.GetCell(column));

        return TypeProfile.FromCells(
            cells);
    }

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

    private void ValidateDataPosition(
        int dataRow,
        int column)
    {
        ValidateColumn(column);

        if (dataRow < 0 ||
            dataRow >= DataRowCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataRow));
        }
    }
}