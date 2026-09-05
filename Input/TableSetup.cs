using ExcelClone.Models;

namespace ExcelClone.Input;

internal static class TableSetup
{
    private const int MinRows = 1;
    private const int MaxRows = 12;

    private const int MinColumns = 1;
    private const int MaxColumns = 8;

    private enum Selection
    {
        Columns,
        Rows
    }

    public static TableSize Create()
    {
        int rows = 4;
        int columns = 3;

        Selection selected =
            Selection.Columns;

        while (true)
        {
            Render(
                rows,
                columns,
                selected);

            ConsoleKey key =
                Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:

                    selected =
                        Selection.Columns;

                    break;

                case ConsoleKey.DownArrow:

                    selected =
                        Selection.Rows;

                    break;

                case ConsoleKey.LeftArrow:

                    ChangeSelectedValue(
                        ref rows,
                        ref columns,
                        selected,
                        -1);

                    break;

                case ConsoleKey.RightArrow:

                    ChangeSelectedValue(
                        ref rows,
                        ref columns,
                        selected,
                        1);

                    break;

                case ConsoleKey.Enter:

                    return new TableSize(
                        rows,
                        columns,
                        false);

                case ConsoleKey.Escape:

                    return new TableSize(
                        0,
                        0,
                        true);
            }
        }
    }

    // ============================================
    // RENDER
    // ============================================

    private static void Render(
        int rows,
        int columns,
        Selection selected)
    {
        Console.Clear();

        Console.WriteLine(
            "============================================");

        Console.WriteLine(
            "                 CREATE TABLE");

        Console.WriteLine(
            "============================================");

        Console.WriteLine();

        Console.WriteLine(
            "↑ / ↓  Select Columns or Rows");

        Console.WriteLine(
            "← / →  Change selected value");

        Console.WriteLine(
            "Enter   Create table");

        Console.WriteLine(
            "Esc     Quit");

        Console.WriteLine();

        DrawSelector(
            "Columns",
            columns,
            MinColumns,
            MaxColumns,
            selected == Selection.Columns);

        DrawSelector(
            "Rows",
            rows,
            MinRows,
            MaxRows,
            selected == Selection.Rows);
    }

    // ============================================
    // VALUE CHANGES
    // ============================================

    private static void ChangeSelectedValue(
        ref int rows,
        ref int columns,
        Selection selected,
        int direction)
    {
        if (selected == Selection.Columns)
        {
            columns =
                Math.Clamp(
                    columns + direction,
                    MinColumns,
                    MaxColumns);

            return;
        }

        rows =
            Math.Clamp(
                rows + direction,
                MinRows,
                MaxRows);
    }

    // ============================================
    // SELECTOR
    // ============================================

    private static void DrawSelector(
        string label,
        int value,
        int minimum,
        int maximum,
        bool selected)
    {
        string marker =
            selected
                ? ">"
                : " ";

        Console.WriteLine(
            $"{marker} {label,-10}[ {value,2} ]   ({minimum}-{maximum})");
    }
}