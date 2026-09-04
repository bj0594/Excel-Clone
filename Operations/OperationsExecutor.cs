using System;
using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Operations;

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

        /*
         * Numeric columns:
         *
         * 0 = Low → High
         * 1 = High → Low
         * 2 = Sum
         */
        if (profile.IsNumeric)
        {
            switch (operation)
            {
                case 0:

                    SortColumn(
                        table,
                        column,
                        profile.DominantType,
                        true);

                    return false;

                case 1:

                    SortColumn(
                        table,
                        column,
                        profile.DominantType,
                        false);

                    return false;

                case 2:

                    return TrySum(
                        table,
                        column,
                        out result);

                default:

                    return false;
            }
        }

        /*
         * Date columns:
         *
         * 0 = Old → New
         * 1 = New → Old
         */
        if (profile.DominantType ==
            DetectedType.DateTime)
        {
            switch (operation)
            {
                case 0:

                    SortColumn(
                        table,
                        column,
                        DetectedType.DateTime,
                        true);

                    return false;

                case 1:

                    SortColumn(
                        table,
                        column,
                        DetectedType.DateTime,
                        false);

                    return false;

                default:

                    return false;
            }
        }

        /*
         * Boolean columns:
         *
         * 0 = True first
         * 1 = False first
         */
        if (profile.DominantType ==
            DetectedType.Bool)
        {
            switch (operation)
            {
                case 0:

                    SortColumn(
                        table,
                        column,
                        DetectedType.Bool,
                        false);

                    return false;

                case 1:

                    SortColumn(
                        table,
                        column,
                        DetectedType.Bool,
                        true);

                    return false;

                default:

                    return false;
            }
        }

        /*
         * String columns:
         *
         * 0 = A → Z
         * 1 = Z → A
         */
        if (profile.DominantType ==
            DetectedType.String)
        {
            switch (operation)
            {
                case 0:

                    SortColumn(
                        table,
                        column,
                        DetectedType.String,
                        true);

                    return false;

                case 1:

                    SortColumn(
                        table,
                        column,
                        DetectedType.String,
                        false);

                    return false;

                default:

                    return false;
            }
        }

        return false;
    }

    // ============================================
    // SORTING
    // ============================================

    private static void SortColumn(
        Table table,
        int column,
        DetectedType type,
        bool ascending)
    {
        /*
         * The table has a maximum of 12 data rows,
         * so a simple bubble sort is sufficient.
         *
         * Entire rows are swapped, not just the
         * selected cell.
         *
         * Empty cells are always moved to the
         * bottom of the table.
         */
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

                /*
                 * Both are empty:
                 * nothing to do.
                 */
                if (left.IsEmpty &&
                    right.IsEmpty)
                {
                    continue;
                }

                /*
                 * Left is empty and right is not.
                 *
                 * Move the empty row DOWN.
                 *
                 * This is the important case that
                 * was reversed in the previous version.
                 */
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

                /*
                 * Right is empty and left is not.
                 *
                 * Leave it where it is because empty
                 * values belong at the bottom.
                 */
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

                if (shouldSwap)
                {
                    SwapRows(
                        table,
                        j,
                        j + 1);

                    swapped = true;
                }
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
        switch (type)
        {
            case DetectedType.Int:
            case DetectedType.Double:

                if (left.TryGetDecimal(
                        out decimal leftNumber) &&
                    right.TryGetDecimal(
                        out decimal rightNumber))
                {
                    return leftNumber.CompareTo(
                        rightNumber);
                }

                break;

            case DetectedType.String:

                return string.Compare(
                    left.DisplayValue,
                    right.DisplayValue,
                    StringComparison.OrdinalIgnoreCase);

            case DetectedType.Bool:

                if (left.RawValue is bool leftBool &&
                    right.RawValue is bool rightBool)
                {
                    return leftBool.CompareTo(
                        rightBool);
                }

                break;

            case DetectedType.DateTime:

                if (left.RawValue is DateTime leftDate &&
                    right.RawValue is DateTime rightDate)
                {
                    return leftDate.CompareTo(
                        rightDate);
                }

                break;
        }

        /*
         * Fallback for unexpected mixed values.
         */
        return string.Compare(
            left.DisplayValue,
            right.DisplayValue,
            StringComparison.OrdinalIgnoreCase);
    }

    // ============================================
    // SUM
    // ============================================

    private static bool TrySum(
        Table table,
        int column,
        out string result)
    {
        result = string.Empty;

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
            return false;
        }

        result =
            sum.ToString(
                "G",
                CultureInfo.InvariantCulture);

        return true;
    }
}