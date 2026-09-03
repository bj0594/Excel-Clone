namespace ExcelClone.Input;

using ExcelClone.Models;

internal static class TableSetup
{
    private const int MinRows = 1;
    private const int MaxRows = 12;

    private const int MinColumns = 1;
    private const int MaxColumns = 8;

    public static TableSize Create()
    {
        int rows = 4;

        int columns = 3;

        // 0 = Columns
        // 1 = Rows
        int selected = 0;

        while (true)
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
                selected == 0);

            DrawSelector(
                "Rows",
                rows,
                MinRows,
                MaxRows,
                selected == 1);

            ConsoleKey key =
                Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:

                    selected = 0;

                    break;

                case ConsoleKey.DownArrow:

                    selected = 1;

                    break;

                case ConsoleKey.LeftArrow:

                    if (selected == 0)
                    {
                        columns =
                            Math.Max(
                                columns - 1,
                                MinColumns);
                    }
                    else
                    {
                        rows =
                            Math.Max(
                                rows - 1,
                                MinRows);
                    }

                    break;

                case ConsoleKey.RightArrow:

                    if (selected == 0)
                    {
                        columns =
                            Math.Min(
                                columns + 1,
                                MaxColumns);
                    }
                    else
                    {
                        rows =
                            Math.Min(
                                rows + 1,
                                MaxRows);
                    }

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