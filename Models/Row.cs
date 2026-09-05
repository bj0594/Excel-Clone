using System;
using System.Collections.Generic;
using ExcelClone.Data;

namespace ExcelClone.Models;

// Represents one row and its cells.
internal sealed class Row
{
    // Rows have a fixed number of columns, so indexed access
    // through a List keeps the implementation simple.
    private readonly List<ICell> cells;

    public Row(
        int columnCount)
    {
        if (columnCount < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columnCount));
        }

        cells =
            new List<ICell>(
                columnCount);

        // Empty cells are created through the factory so that
        // Row does not need to know which Cell<T> implementation
        // is used internally.
        for (int column = 0;
             column < columnCount;
             column++)
        {
            cells.Add(
                CellFactory.Create(
                    string.Empty));
        }
    }

    public ICell GetCell(
        int column)
    {
        ValidateColumn(
            column);

        return cells[column];
    }

    public void SetCell(
        int column,
        ICell cell)
    {
        ValidateColumn(
            column);

        ArgumentNullException.ThrowIfNull(
            cell);

        cells[column] =
            cell;
    }

    private void ValidateColumn(
        int column)
    {
        if (column < 0 ||
            column >= cells.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }
    }
}