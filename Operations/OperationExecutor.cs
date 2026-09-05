using System;
using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Operations;

// Executes the operations available for a table column.
//
// Sorting moves complete rows so that values in different
// columns remain associated with the same record.
internal static class OperationExecutor
{
    public static bool TryExecute(
        Table table,
        int column,
        int operation,
        out string result)
    {
        result = string.Empty;

        if (column < 0 ||
            column >= table.ColumnCount)
        {
            return false;
        }

        TypeProfile profile =
            table.AnalyzeColumn(
                column);

        if (profile.DominantType ==
            DetectedType.Empty)
        {
            return false;
        }

        if (profile.IsNumeric)
        {
            if (operation == 2)
            {
                return TrySum(
                    table,
                    column,
                    out result);
            }

            if (operation == 0 ||
                operation == 1)
            {
                SortColumn(
                    table,
                    column,
                    profile.DominantType,
                    operation == 0);

                return false;
            }

            return false;
        }

        if (operation != 0 &&
            operation != 1)
        {
            return false;
        }

        bool ascending =
            operation == 0;

        // bool.CompareTo places false before true,
        // while the UI defines "True first" as the
        // first boolean operation.
        if (profile.DominantType ==
            DetectedType.Bool)
        {
            ascending = !ascending;
        }

        switch (profile.DominantType)
        {
            case DetectedType.String:
            case DetectedType.Bool:
            case DetectedType.DateTime:

                SortColumn(
                    table,
                    column,
                    profile.DominantType,
                    ascending);

                return false;

            default:

                return false;
        }
    }

    // Sorts complete rows rather than individual cells.
    // Empty cells are always placed at the bottom.
    private static void SortColumn(
        Table table,
        int column,
        DetectedType type,
        bool ascending)
    {
        // The table is deliberately limited to 12 rows,
        // so bubble sort is sufficient and keeps the
        // sorting logic easy to understand.
        for (int i = 0;
             i < table.DataRowCount - 1;
             i++)
        {
            bool swapped = false;

            for (int j = 0;
                 j < table.DataRowCount - 1 - i;
                 j++)
            {
                ICell left =
                    table.Rows[j]
                        .GetCell(
                            column);

                ICell right =
                    table.Rows[j + 1]
                        .GetCell(
                            column);

                if (left.IsEmpty &&
                    right.IsEmpty)
                {
                    continue;
                }

                if (left.IsEmpty &&
                    !right.IsEmpty)
                {
                    SwapRows(
                        table,
                        j,
                        j + 1);

                    swapped = true;

                    continue;
                }

                if (!left.IsEmpty &&
                    right.IsEmpty)
                {
                    continue;
                }

                int comparison =
                    CompareCells(
                        left,
                        right,
                        type);

                bool shouldSwap =
                    ascending
                        ? comparison > 0
                        : comparison < 0;

                if (!shouldSwap)
                {
                    continue;
                }

                SwapRows(
                    table,
                    j,
                    j + 1);

                swapped = true;
            }

            if (!swapped)
            {
                break;
            }
        }
    }

    private static void SwapRows(
        Table table,
        int firstRow,
        int secondRow)
    {
        Row first =
            table.Rows[
                firstRow];

        Row second =
            table.Rows[
                secondRow];

        for (int column = 0;
             column < table.ColumnCount;
             column++)
        {
            ICell firstCell =
                first.GetCell(
                    column);

            ICell secondCell =
                second.GetCell(
                    column);

            first.SetCell(
                column,
                secondCell);

            second.SetCell(
                column,
                firstCell);
        }
    }

    private static int CompareCells(
        ICell left,
        ICell right,
        DetectedType type)
    {
        return type switch
        {
            DetectedType.Int or
            DetectedType.Double =>
                CompareNumbers(
                    left,
                    right),

            DetectedType.String =>
                string.Compare(
                    left.DisplayValue,
                    right.DisplayValue,
                    StringComparison.OrdinalIgnoreCase),

            DetectedType.Bool =>
                CompareBooleans(
                    left,
                    right),

            DetectedType.DateTime =>
                CompareDates(
                    left,
                    right),

            _ =>
                0
        };
    }

    private static int CompareNumbers(
        ICell left,
        ICell right)
    {
        if (left.TryGetDecimal(
                out decimal leftNumber) &&
            right.TryGetDecimal(
                out decimal rightNumber))
        {
            return leftNumber.CompareTo(
                rightNumber);
        }

        return 0;
    }

    private static int CompareBooleans(
        ICell left,
        ICell right)
    {
        if (left.RawValue is bool leftValue &&
            right.RawValue is bool rightValue)
        {
            return leftValue.CompareTo(
                rightValue);
        }

        return 0;
    }

    private static int CompareDates(
        ICell left,
        ICell right)
    {
        if (left.RawValue is DateTime leftValue &&
            right.RawValue is DateTime rightValue)
        {
            return leftValue.CompareTo(
                rightValue);
        }

        return 0;
    }

    // Calculates the sum of all numeric cells in a column.
    private static bool TrySum(
        Table table,
        int column,
        out string result)
    {
        decimal sum = 0;
        bool foundNumber = false;

        for (int dataRow = 0;
             dataRow < table.DataRowCount;
             dataRow++)
        {
            ICell cell =
                table.GetDataCell(
                    dataRow,
                    column);

            if (!cell.TryGetDecimal(
                    out decimal value))
            {
                continue;
            }

            sum += value;
            foundNumber = true;
        }

        if (!foundNumber)
        {
            result = string.Empty;
            return false;
        }

        result =
            sum.ToString(
                "G",
                CultureInfo.InvariantCulture);

        return true;
    }
}