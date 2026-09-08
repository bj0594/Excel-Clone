using System.Text;
using ExcelClone.Input;
using ExcelClone.Models;

namespace ExcelClone;

// Entry point for the Excel Clone application.
// Program is intentionally kept small and delegates
// table setup, editing and application logic to other classes.
internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding =
            Encoding.UTF8;

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