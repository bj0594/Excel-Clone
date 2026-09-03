using System;
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

        /*
         * Operation 2 = Sum
         *
         * Sum is only valid for a numeric
         * dominant column.
         */
        if (operation != 2 ||
            !profile.IsNumeric)
        {
            return false;
        }

        decimal sum = 0;

        for (int dataRow = 0;
             dataRow < table.DataRowCount;
             dataRow++)
        {
            ICell cell =
                table.GetDataCell(
                    dataRow,
                    column);

            if (cell.TryGetDecimal(
                    out decimal value))
            {
                sum += value;
            }
        }

        result =
            FormatNumber(
                sum);

        return true;
    }

    private static string FormatNumber(
        decimal value)
    {
        return value.ToString(
            "G",
            System.Globalization.CultureInfo.InvariantCulture);
    }
}