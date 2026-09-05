using System;
using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Operations;

// Executes operations selected in the operation block.
//
// The executor contains the behaviour of the available
// column operations, while rendering and input handling
// remain in their own classes.
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

        // Numeric columns have three operations:
        //
        // 0 = Lowest → Highest
        // 1 = Highest → Lowest
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
                bool ascending =
                    operation == 0;

                SortColumn(
                    table,
                    column,
                    profile.DominantType,
                    ascending);

                return false;
            }

            return false;
        }

        // All other supported types have two sorting operations.
        //
        // String:
        // 0 = A → Z
        // 1 = Z → A
        //
        // DateTime:
        // 0 = Oldest → Newest
        // 1 = Newest → Oldest
        //
        // Bool:
        // 0 = True first
        // 1 = False first
        if (operation != 0 &&
            operation != 1)
        {
            return false;
        }

        bool sortAscending =
            operation == 0;

        // Boolean ordering is intentionally reversed because
        // bool.CompareTo places false before true, while the
        // user-facing operation list defines "True first".
        if (profile.DominantType ==
            DetectedType.Bool)
        {
            sortAscending =
                !sortAscending;
        }

        switch (profile.DominantType)
        {
            case DetectedType.DateTime:
            case DetectedType.Bool:
            case DetectedType.String:

                SortColumn(
                    table,
                    column,
                    profile.DominantType,
                    sortAscending);

                return false;

            default:

                return false;
        }
    }

    // ============================================
    // SORTING
    // ============================================

    // Sorts complete rows rather than individual cells.
    //
    // This is important because values in different columns
    // belong to the same row and must remain associated.
    private static void SortColumn(
        Table table,
        int column,
        DetectedType type,
        bool ascending)
    {
        // The table supports a maximum of 12 data rows.
        // Bubble sort is therefore simple and entirely adequate
        // for the application's deliberately small data set.
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

                // Empty cells always belong at the bottom.
                if (left.IsEmpty &&
                    right.IsEmpty)
                {
                    continue;
                }

                // Move an empty value down whenever it is
                // directly above a populated value.
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

                // The right cell is empty, so the populated
                // left cell is already in the correct position.
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

            // Stop early if the current pass made no changes.
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

        // Swap every cell so the complete rows remain intact.
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
                string.Compare(
                    left.DisplayValue,
                    right.DisplayValue,
                    StringComparison.OrdinalIgnoreCase)
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

        // Both cells are expected to contain the dominant
        // column type. If conversion unexpectedly fails,
        // leave their relative order unchanged.
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

    // Calculates the sum of all numeric cells in a column.
    //
    // TryGetDecimal allows both int and double cells to use
    // the same calculation without duplicating the summing
    // logic for each numeric type.
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