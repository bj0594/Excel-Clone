using System;
using System.Collections.Generic;
using System.Linq;
using ExcelClone.Models;

namespace ExcelClone.Rendering;

internal static class TableRenderer
{
    private const int CellWidth = 15;

    private const int BorderSegmentWidth =
        CellWidth + 2;

    private static readonly ConsoleColor HeaderBorderColor =
        ConsoleColor.Cyan;

    public static void Render(
        Table table,
        int activeRow,
        int activeColumn)
    {
        Render(
            table,
            activeRow,
            activeColumn,
            null,
            false,
            false,
            0);
    }

    public static void Render(
        Table table,
        int activeRow,
        int activeColumn,
        string? editingValue,
        bool editing,
        bool operationFocus,
        int activeOperation)
    {
        Console.ResetColor();

        // ========================================
        // HEADER TOP
        // ========================================

        Console.ForegroundColor =
            HeaderBorderColor;

        Console.WriteLine(
            BuildTopBorder(
                table.ColumnCount));

        Console.ResetColor();

        // ========================================
        // HEADER CONTENT
        // ========================================

        RenderHeaderRow(
            table,
            operationFocus
                ? -1
                : activeRow,
            activeColumn,
            editingValue,
            editing);

        // ========================================
        // HEADER BOTTOM
        // ========================================

        Console.ForegroundColor =
            HeaderBorderColor;

        Console.WriteLine(
            BuildBottomBorder(
                table.ColumnCount));

        Console.ResetColor();

        // ========================================
        // DATA TOP
        // ========================================

        Console.WriteLine(
            BuildTopBorder(
                table.ColumnCount));

        // ========================================
        // DATA ROWS
        // ========================================

        for (int dataRow = 0;
             dataRow < table.DataRowCount;
             dataRow++)
        {
            int displayRow =
                dataRow + 1;

            RenderDataRow(
                table,
                dataRow,
                displayRow,
                operationFocus
                    ? -1
                    : activeRow,
                activeColumn,
                editingValue,
                editing);

            if (dataRow <
                table.DataRowCount - 1)
            {
                RenderDataMiddleBorder(
                    table.ColumnCount);
            }
        }

        // ========================================
        // DATA BOTTOM
        // ========================================

        RenderDataBottomBorder(
            table.ColumnCount);

        // ========================================
        // OPERATION BLOCK
        // ========================================

        OperationRenderer.Render(
            table,
            operationFocus
                ? activeColumn
                : -1,
            activeOperation);

        Console.ResetColor();
    }

    // ============================================
    // HEADER
    // ============================================

    private static void RenderHeaderRow(
        Table table,
        int activeRow,
        int activeColumn,
        string? editingValue,
        bool editing)
    {
        Console.ForegroundColor =
            HeaderBorderColor;

        Console.Write(
            "│");

        Console.ResetColor();

        for (int column = 0;
             column < table.ColumnCount;
             column++)
        {
            bool active =
                activeRow == 0 &&
                activeColumn == column;

            string header =
                table.GetHeader(
                    column);

            bool hasHeader =
                !string.IsNullOrEmpty(
                    header);

            if (active &&
                editing &&
                editingValue != null)
            {
                header =
                    editingValue;

                hasHeader =
                    !string.IsNullOrEmpty(
                        header);
            }

            if (!hasHeader &&
                !(active && editing))
            {
                header =
                    $"Header {column + 1}";
            }

            RenderHeaderCellContent(
                header,
                active,
                !hasHeader &&
                !(active && editing));

            if (column <
                table.ColumnCount - 1)
            {
                Console.ForegroundColor =
                    HeaderBorderColor;

                Console.Write(
                    "│");

                Console.ResetColor();
            }
        }

        Console.ForegroundColor =
            HeaderBorderColor;

        Console.WriteLine(
            "│");

        Console.ResetColor();
    }

    private static void RenderHeaderCellContent(
        string value,
        bool active,
        bool placeholder)
    {
        string marker =
            active
                ? "> "
                : string.Empty;

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

        Console.Write(
            " ");

        if (placeholder &&
            !active)
        {
            Console.ForegroundColor =
                ConsoleColor.DarkGray;
        }
        else
        {
            Console.ForegroundColor =
                ConsoleColor.White;
        }

        Console.Write(
            text);

        Console.ResetColor();

        Console.Write(
            " ");
    }

    // ============================================
    // DATA
    // ============================================

    private static void RenderDataRow(
        Table table,
        int dataRow,
        int displayRow,
        int activeRow,
        int activeColumn,
        string? editingValue,
        bool editing)
    {
        Console.ResetColor();

        Console.Write(
            "│");

        for (int column = 0;
             column < table.ColumnCount;
             column++)
        {
            ICell cell =
                table.GetDataCell(
                    dataRow,
                    column);

            bool active =
                activeRow == displayRow &&
                activeColumn == column;

            string value =
                active &&
                editing &&
                editingValue != null
                    ? editingValue
                    : cell.DisplayValue;

            RenderDataCellContent(
                value,
                active);

            if (column <
                table.ColumnCount - 1)
            {
                Console.Write(
                    "│");
            }
        }

        Console.WriteLine(
            "│");

        Console.ResetColor();
    }

    private static void RenderDataCellContent(
        string value,
        bool active)
    {
        string marker =
            active
                ? "> "
                : string.Empty;

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

        Console.Write(
            " " +
            text +
            " ");
    }

    // ============================================
    // DATA BORDERS
    // ============================================

    private static void RenderDataMiddleBorder(
        int columnCount)
    {
        Console.ResetColor();

        Console.WriteLine(
            BuildMiddleBorder(
                columnCount));
    }

    private static void RenderDataBottomBorder(
        int columnCount)
    {
        Console.ResetColor();

        Console.WriteLine(
            BuildBottomBorder(
                columnCount));
    }

    // ============================================
    // BORDER BUILDERS
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

    private static string BuildMiddleBorder(
        int columnCount)
    {
        string segment =
            new string(
                '─',
                BorderSegmentWidth);

        return
            "├" +
            string.Join(
                "┼",
                Enumerable.Repeat(
                    segment,
                    columnCount)) +
            "┤";
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

        return value[..(width - 3)] +
            "...";
    }
}