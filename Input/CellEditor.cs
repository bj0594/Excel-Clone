using ExcelClone.Data;
using ExcelClone.Models;
using ExcelClone.Rendering;

namespace ExcelClone.Input;

internal static class CellEditor
{
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

        RenderEditingState(
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

                    return EditResult.Stay;

                case ConsoleKey.Escape:

                    return EditResult.Stay;

                case ConsoleKey.UpArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveUp;

                case ConsoleKey.DownArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveDown;

                case ConsoleKey.LeftArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveLeft;

                case ConsoleKey.RightArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveRight;

                case ConsoleKey.Backspace:

                    if (buffer.Length > 0)
                    {
                        buffer =
                            buffer[..^1];

                        RenderEditingState(
                            table,
                            displayRow,
                            column,
                            buffer);
                    }

                    break;

                default:

                    if (!char.IsControl(
                            key.KeyChar))
                    {
                        buffer +=
                            key.KeyChar;

                        RenderEditingState(
                            table,
                            displayRow,
                            column,
                            buffer);
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

    private static void RenderEditingState(
        Table table,
        int displayRow,
        int column,
        string buffer)
    {
        Console.Clear();

        Console.SetCursorPosition(
            0,
            0);

        TableRenderer.Render(
            table,
            displayRow,
            column,
            buffer,
            true);

        Console.SetCursorPosition(
            CalculateInputCursorX(
                column),
            CalculateInputCursorY(
                displayRow));

        Console.CursorVisible = true;
    }

    private static int CalculateInputCursorX(
        int column)
    {
        const int cellWidth = 15;

        return
            column *
            (cellWidth + 3) +
            3;
    }

    private static int CalculateInputCursorY(
        int displayRow)
    {
        // 0 = header top border
        // 1 = header content
        // 2 = header bottom border
        // 3 = data top border
        // 4 = first data row

        return
            displayRow == 0
                ? 1
                : displayRow * 2 + 2;
    }
}