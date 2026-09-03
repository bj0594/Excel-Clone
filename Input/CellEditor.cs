using ExcelClone.Data;
using ExcelClone.Models;
using ExcelClone.Rendering;

namespace ExcelClone.Input;

internal static class CellEditor
{
    private const int CellWidth = 15;

    public static EditResult Edit(
        Table table,
        int displayRow,
        int column,
        char? firstCharacter,
        bool deleteLastCharacter = false)
    {
        bool isHeader =
            displayRow == 0;

        string originalValue =
            isHeader
                ? table.GetHeader(column)
                : table
                    .GetDataCell(
                        displayRow - 1,
                        column)
                    .DisplayValue;

        string buffer =
            originalValue;

        /*
         * Direct typing replaces the existing value.
         */
        if (firstCharacter.HasValue)
        {
            buffer =
                firstCharacter.Value.ToString();
        }

        /*
         * Backspace starts editing immediately
         * and removes the last existing character.
         */
        if (deleteLastCharacter &&
            buffer.Length > 0)
        {
            buffer =
                buffer[..^1];
        }

        RenderInitialEditingState(
            table,
            displayRow,
            column,
            buffer);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    Console.CursorVisible = false;

                    return EditResult.Stay;

                case ConsoleKey.Escape:

                    RenderNormalState(
                        table,
                        displayRow,
                        column);

                    return EditResult.Stay;

                case ConsoleKey.UpArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    Console.CursorVisible = false;

                    return EditResult.MoveUp;

                case ConsoleKey.DownArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    Console.CursorVisible = false;

                    return EditResult.MoveDown;

                case ConsoleKey.LeftArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    Console.CursorVisible = false;

                    return EditResult.MoveLeft;

                case ConsoleKey.RightArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    Console.CursorVisible = false;

                    return EditResult.MoveRight;

                case ConsoleKey.Backspace:

                    if (buffer.Length > 0)
                    {
                        buffer =
                            buffer[..^1];

                        UpdateEditingCell(
                            buffer,
                            displayRow,
                            column);
                    }

                    break;

                default:

                    if (!char.IsControl(
                            key.KeyChar))
                    {
                        buffer +=
                            key.KeyChar;

                        UpdateEditingCell(
                            buffer,
                            displayRow,
                            column);
                    }

                    break;
            }
        }
    }

    private static void Commit(
        Table table,
        int displayRow,
        int column,
        string buffer)
    {
        Console.CursorVisible = false;

        if (displayRow == 0)
        {
            table.SetHeader(
                column,
                buffer);

            return;
        }

        ICell cell =
            CellFactory.Create(
                buffer);

        table.SetDataCell(
            displayRow - 1,
            column,
            cell);
    }

    private static void RenderInitialEditingState(
        Table table,
        int displayRow,
        int column,
        string buffer)
    {
        Console.CursorVisible = false;

        Console.Clear();

        Console.SetCursorPosition(
            0,
            0);

        /*
         * We are editing a table cell, so operation
         * focus is explicitly disabled.
         *
         * This matches the current 7-parameter
         * TableRenderer.Render() signature.
         */
        TableRenderer.Render(
            table,
            displayRow,
            column,
            buffer,
            true,
            false,
            0);

        PositionCursor(
            displayRow,
            column,
            buffer);

        Console.CursorVisible = true;
    }

    private static void UpdateEditingCell(
        string buffer,
        int displayRow,
        int column)
    {
        int x =
            CalculateInputCursorX(
                column);

        int y =
            CalculateInputCursorY(
                displayRow);

        Console.SetCursorPosition(
            x,
            y);

        string visibleText =
            Fit(
                buffer,
                CellWidth);

        Console.Write(
            visibleText.PadRight(
                CellWidth));

        int cursorOffset =
            Math.Min(
                buffer.Length,
                CellWidth);

        Console.SetCursorPosition(
            x + cursorOffset,
            y);

        Console.CursorVisible = true;
    }

    private static void PositionCursor(
        int displayRow,
        int column,
        string buffer)
    {
        int x =
            CalculateInputCursorX(
                column);

        int y =
            CalculateInputCursorY(
                displayRow);

        int cursorOffset =
            Math.Min(
                buffer.Length,
                CellWidth);

        Console.SetCursorPosition(
            x + cursorOffset,
            y);
    }

    private static void RenderNormalState(
        Table table,
        int displayRow,
        int column)
    {
        Console.CursorVisible = false;

        Console.Clear();

        Console.SetCursorPosition(
            0,
            0);

        /*
         * Normal table rendering:
         * operation focus is off.
         */
        TableRenderer.Render(
            table,
            displayRow,
            column,
            null,
            false,
            false,
            0);

        Console.CursorVisible = false;
    }

    private static int CalculateInputCursorX(
        int column)
    {
        return
            column *
            (CellWidth + 3) +
            3;
    }

    private static int CalculateInputCursorY(
        int displayRow)
    {
        return
            displayRow == 0
                ? 1
                : displayRow * 2 + 2;
    }

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