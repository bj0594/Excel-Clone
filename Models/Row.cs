using ExcelClone.Data;

namespace ExcelClone.Models;

internal sealed class Row
{
    private readonly List<ICell> cells;

    public Row(
        int columnCount)
    {
        cells =
            new List<ICell>(
                columnCount);

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
        if (column < 0 ||
            column >= cells.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }

        return cells[column];
    }

    public void SetCell(
        int column,
        ICell cell)
    {
        if (column < 0 ||
            column >= cells.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }

        cells[column] =
            cell;
    }
}