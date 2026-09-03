using System;
using System.Collections.Generic;
using System.Linq;
using ExcelClone.Models;

namespace ExcelClone.Rendering;

internal static class OperationRenderer
{
    private const int CellWidth = 15;

    private const int BorderSegmentWidth =
        CellWidth + 2;

    private const int OperationRowCount = 4;

    public static void Render(
        Table table,
        int activeColumn,
        int activeOperation,
        string? operationResult = null)
    {
        Console.ResetColor();

        // ========================================
        // OPERATION TOP
        // ========================================

        Console.WriteLine(
            BuildTopBorder(
                table.ColumnCount));

        // ========================================
        // OPERATION ROWS
        // ========================================

        for (int operationRow = 0;
             operationRow < OperationRowCount;
             operationRow++)
        {
            RenderOperationRow(
                table,
                activeColumn,
                activeOperation,
                operationRow,
                operationResult);
        }

        // ========================================
        // OPERATION BOTTOM
        // ========================================

        Console.WriteLine(
            BuildBottomBorder(
                table.ColumnCount));

        Console.ResetColor();
    }

    private static void RenderOperationRow(
        Table table,
        int activeColumn,
        int activeOperation,
        int operationRow,
        string? operationResult)
    {
        Console.Write("│");

        for (int column = 0;
             column < table.ColumnCount;
             column++)
        {
            TypeProfile profile =
                table.AnalyzeColumn(
                    column);

            IReadOnlyList<string> operations =
                GetOperations(
                    profile);

            string operation =
                operationRow <
                operations.Count
                    ? operations[operationRow]
                    : string.Empty;

            bool selected =
                column == activeColumn &&
                operationRow == activeOperation &&
                !string.IsNullOrEmpty(
                    operation);

            /*
             * Show the execution result in the
             * selected Sum cell.
             */
            string displayValue =
                operation;

            if (selected &&
                operationRow == 2 &&
                operationResult != null)
            {
                displayValue =
                    $"Sum: {operationResult}";
            }

            RenderOperationCell(
                displayValue,
                selected);

            if (column <
                table.ColumnCount - 1)
            {
                Console.Write("│");
            }
        }

        Console.WriteLine("│");
    }

    private static void RenderOperationCell(
        string value,
        bool selected)
    {
        string marker =
            selected
                ? "> "
                : "  ";

        int availableWidth =
            CellWidth -
            marker.Length;

        string content =
            Fit(
                value,
                availableWidth);

        string text =
            marker +
            content;

        text =
            text.PadRight(
                CellWidth);

        Console.Write(" ");

        Console.ForegroundColor =
            selected
                ? ConsoleColor.White
                : ConsoleColor.DarkGray;

        Console.Write(
            text);

        Console.ResetColor();

        Console.Write(" ");
    }

    private static IReadOnlyList<string>
        GetOperations(
            TypeProfile profile)
    {
        // ========================================
        // NUMERIC
        // ========================================

        if (profile.IsNumeric)
        {
            return
            [
                "Low → High",
                "High → Low",
                "Sum"
            ];
        }

        // ========================================
        // DATE
        // ========================================

        switch (profile.DominantType)
        {
            case DetectedType.DateTime:

                return
                [
                    "Old → New",
                    "New → Old"
                ];

            // ====================================
            // BOOLEAN
            // ====================================

            case DetectedType.Bool:

                return
                [
                    "True first",
                    "False first"
                ];

            // ====================================
            // STRING
            // ====================================

            case DetectedType.String:

                return
                [
                    "A → Z",
                    "Z → A"
                ];

            // ====================================
            // EMPTY
            // ====================================

            case DetectedType.Empty:

                return
                [
                    "No type found"
                ];

            default:

                return
                [
                    "No operations"
                ];
        }
    }

    // ============================================
    // BORDERS
    // ============================================

    private static string BuildTopBorder(
        int columnCount)
    {
        string segment =
            new string(
                '─',
                BorderSegmentWidth);

        return
            "┌" +
            string.Join(
                "┬",
                Enumerable.Repeat(
                    segment,
                    columnCount)) +
            "┐";
    }

    private static string BuildBottomBorder(
        int columnCount)
    {
        string segment =
            new string(
                '─',
                BorderSegmentWidth);

        return
            "└" +
            string.Join(
                "┴",
                Enumerable.Repeat(
                    segment,
                    columnCount)) +
            "┘";
    }

    // ============================================
    // SHARED
    // ============================================

    private static string Fit(
        string value,
        int width)
    {
        if (value.Length <= width)
        {
            return value;
        }

        if (width <= 3)
        {
            return value[..width];
        }

        return
            value[..(width - 3)] +
            "...";
    }
}