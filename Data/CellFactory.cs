using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Data;

// Converts text entered by the user into the most
// appropriate strongly typed Cell<T>.
//
// The order of the checks matters. More specific
// types are tested before falling back to string.
internal static class CellFactory
{
    public static ICell Create(
        string input)
    {
        // An empty input becomes an empty string cell.
        if (string.IsNullOrEmpty(input))
        {
            return new Cell<string>(
                string.Empty);
        }

        // Integers are checked before doubles so that
        // values such as "42" become Cell<int>.
        if (int.TryParse(
                input,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int intValue))
        {
            return new Cell<int>(
                intValue);
        }

        // Decimal-point numbers become Cell<double>.
        if (double.TryParse(
                input,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double doubleValue))
        {
            return new Cell<double>(
                doubleValue);
        }

        // Boolean values such as "true" and "false"
        // become Cell<bool>.
        if (bool.TryParse(
                input,
                out bool boolValue))
        {
            return new Cell<bool>(
                boolValue);
        }

        // First support the spreadsheet's explicit
        // Norwegian-style date format.
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

        // Also allow other unambiguous date formats
        // supported by the invariant culture.
        if (DateTime.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dateValue))
        {
            return new Cell<DateTime>(
                dateValue);
        }

        // Anything that does not match a supported type
        // is treated as ordinary text.
        return new Cell<string>(
            input);
    }
}