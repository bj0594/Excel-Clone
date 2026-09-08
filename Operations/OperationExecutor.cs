using System;
using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Operations;

// Executes operations selected in the operation block.
//
// Sorting moves complete rows so values in different columns
// remain associated with the same row.
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

        // Numeric columns provide three operations:
        // 0 = Low → High
        // 1 = High → Low
        // 2 = Sum
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

        // Non-numeric supported types provide two operations.
        if (operation != 0 &&
            operation != 1)
        {
            return false;
        }

        bool ascending =
            operation == 0;

        // Bool has the opposite natural ordering of the
        // user-facing "True first"/"False first" operations.
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

    // ============================================
    // SORTING
    // ============================================

    // Sorts complete rows rather than individual cells.
    // Empty cells are always moved to the bottom.
    private static void SortColumn(
        Table table,
        int column,
        DetectedType type,
        bool ascending)
    {
        // The table contains at most 12 data rows, so bubble
        // sort keeps the implementation simple and readable.
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

        // Swap every cell so the two complete rows stay intact.
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

    // ============================================
    // COMPARISON
    // ============================================

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

    // ============================================
    // SUM
    // ============================================

    // Uses the common numeric conversion supplied by ICell,
    // allowing int and double values to share one calculation.
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