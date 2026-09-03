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
        char? firstCharacter)
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

        string buffer;

        if (firstCharacter.HasValue)
        {
            buffer =
                firstCharacter.Value.ToString();
        }
        else
        {
            buffer =
                originalValue;
        }

        /*
         * Render the complete table only once when
         * editing starts.
         */
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

                    /*
                     * Re-render the original table so that
                     * any temporary editing text disappears.
                     */
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
         * CellEditor always starts editing inside
         * the main table, never inside the operation
         * block.
         */
        TableRenderer.Render(
            table,
            displayRow,
            column,
            buffer,
            true,
            false,
            0);

        /*
         * Move into the active cell.
         *
         * We do not redraw the table after this.
         */
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
        /*
         * Only rewrite the contents of the active
         * cell. The rest of the table is untouched.
         */
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

        /*
         * Put the cursor immediately after the
         * currently visible text.
         */
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
         * After leaving edit mode we return to the
         * normal table view. Operation focus is off.
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
        /*
         * The table uses:
         *
         * │ + space + 15 characters + space
         *
         * So the first character of a cell is
         * two positions after its left border.
         */
        return
            column *
            (CellWidth + 3) +
            3;
    }

    private static int CalculateInputCursorY(
        int displayRow)
    {
        /*
         * TableRenderer currently renders:
         *
         * 0 = header top border
         * 1 = header content
         * 2 = header bottom border
         * 3 = data top border
         * 4 = first data row
         *
         * Data rows therefore start at:
         *
         * displayRow * 2 + 2
         *
         * Header itself is on row 1.
         */
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