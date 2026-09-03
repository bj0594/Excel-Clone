using ExcelClone.Models;
using ExcelClone.Rendering;

namespace ExcelClone.Input;

internal static class TableEditor
{
    public static void Run(Table table)
    {
        int activeRow = 0;

        int activeColumn = 0;

        RenderTable(
            table,
            activeRow,
            activeColumn);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:

                    activeRow =
                        Math.Max(
                            activeRow - 1,
                            0);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.DownArrow:

                    activeRow =
                        Math.Min(
                            activeRow + 1,
                            table.TotalDisplayRows - 1);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.LeftArrow:

                    activeColumn =
                        Math.Max(
                            activeColumn - 1,
                            0);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.RightArrow:

                    activeColumn =
                        Math.Min(
                            activeColumn + 1,
                            table.ColumnCount - 1);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.Enter:

                    EditResult enterResult =
                        CellEditor.Edit(
                            table,
                            activeRow,
                            activeColumn,
                            null);

                    MoveAfterEditing(
                        table,
                        ref activeRow,
                        ref activeColumn,
                        enterResult);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.Escape:

                    return;

                default:

                    if (!char.IsControl(
                            key.KeyChar))
                    {
                        EditResult typingResult =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                key.KeyChar);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            typingResult);

                        RenderTable(
                            table,
                            activeRow,
                            activeColumn);
                    }

                    break;
            }
        }
    }

    private static void MoveAfterEditing(
        Table table,
        ref int activeRow,
        ref int activeColumn,
        EditResult result)
    {
        switch (result)
        {
            case EditResult.MoveUp:

                activeRow =
                    Math.Max(
                        activeRow - 1,
                        0);

                break;

            case EditResult.MoveDown:

                activeRow =
                    Math.Min(
                        activeRow + 1,
                        table.TotalDisplayRows - 1);

                break;

            case EditResult.MoveLeft:

                activeColumn =
                    Math.Max(
                        activeColumn - 1,
                        0);

                break;

            case EditResult.MoveRight:

                activeColumn =
                    Math.Min(
                        activeColumn + 1,
                        table.ColumnCount - 1);

                break;
        }
    }

    private static void RenderTable(
        Table table,
        int activeRow,
        int activeColumn)
    {
        Console.CursorVisible = false;

        Console.SetCursorPosition(
            0,
            0);

        TableRenderer.Render(
            table,
            activeRow,
            activeColumn);
    }
}