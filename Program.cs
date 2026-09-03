using ExcelClone.Input;
using ExcelClone.Models;

namespace ExcelClone;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding =
            System.Text.Encoding.UTF8;

        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();

            ShowWelcome();

            TableSize tableSize =
                TableSetup.Create();

            if (tableSize.Cancelled)
            {
                Console.CursorVisible = true;
                return;
            }

            Table table =
                new Table(
                    tableSize.DataRows,
                    tableSize.Columns);

            TableEditor.Run(table);

            Console.Clear();

            Console.WriteLine(
                "Table closed.");

            Console.WriteLine();

            Console.Write(
                "Create another table? (Y/N): ");

            ConsoleKey key =
                Console.ReadKey(true).Key;

            if (key != ConsoleKey.Y)
            {
                Console.CursorVisible = true;
                return;
            }
        }
    }

    private static void ShowWelcome()
    {
        Console.WriteLine(
            "============================================");

        Console.WriteLine(
            "                 EXCEL CLONE");

        Console.WriteLine(
            "============================================");

        Console.WriteLine();

        Console.WriteLine(
            "A small terminal-based spreadsheet.");

        Console.WriteLine();

        Console.WriteLine(
            "Press any key to continue...");

        Console.ReadKey(true);
    }
}