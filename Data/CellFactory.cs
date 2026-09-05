using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Data;

// Converts user input into a strongly typed Cell<T>.
// The first successful conversion determines the cell type.
internal static class CellFactory
{
    public static ICell Create(
        string input)
    {
        // Empty input represents an empty cell.
        if (string.IsNullOrEmpty(input))
        {
            return new Cell<string>(
                string.Empty);
        }

        // Integers are checked first so values such as "42"
        // are stored as Cell<int> rather than Cell<double>.
        if (int.TryParse(
                input,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int intValue))
        {
            return new Cell<int>(
                intValue);
        }

        // Explicitly support the spreadsheet's Norwegian-style
        // date format: day.month.year.
        if (DateTime.TryParseExact(
                input,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime exactDate))
        {
            return new Cell<DateTime>(
                exactDate);
        }

        // Decimal-point numbers are stored as Cell<double>.
        if (double.TryParse(
                input,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double doubleValue))
        {
            return new Cell<double>(
                doubleValue);
        }

        // Boolean values such as "true" and "false" become
        // Cell<bool>.
        if (bool.TryParse(
                input,
                out bool boolValue))
        {
            return new Cell<bool>(
                boolValue);
        }

        // Also accept other date representations supported
        // by the invariant culture.
        if (DateTime.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dateValue))
        {
            return new Cell<DateTime>(
                dateValue);
        }

        // Values that match none of the supported types
        // remain ordinary text.
        return new Cell<string>(
            input);
    }
}