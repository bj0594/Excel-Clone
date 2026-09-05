using System;
using System.Collections.Generic;
using ExcelClone.Data;

namespace ExcelClone.Models;

// Represents one row in the spreadsheet.
//
// A row stores cells through the ICell interface rather
// than depending on a specific Cell<T> type. This allows
// different generic Cell<T> implementations to coexist
// naturally in the same table.
internal sealed class Row
{
    // The cells belonging to this row.
    //
    // The number of cells is fixed when the row is created,
    // so a List<ICell> is sufficient and keeps indexed access
    // simple.
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

        // Every new cell starts empty.
        //
        // CellFactory is used here so that the creation of
        // cells remains centralized instead of constructing
        // a concrete Cell<T> directly inside Row.
        for (int column = 0;
             column < columnCount;
             column++)
        {
            cells.Add(
                CellFactory.Create(
                    string.Empty));
        }
    }

    // Returns the cell at the specified column.
    public ICell GetCell(
        int column)
    {
        ValidateColumn(
            column);

        return cells[column];
    }

    // Replaces the cell at the specified column.
    //
    // ICell allows the caller to provide any supported
    // generic Cell<T> implementation.
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

    // Keeps index validation in one place so GetCell and
    // SetCell cannot accidentally implement different rules.
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