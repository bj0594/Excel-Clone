using System.Globalization;

namespace ExcelClone.Data;

// Responsible for converting typed cell values into
// the text representation shown in the terminal.
//
// Keeping formatting separate from Cell<T> means that
// the generic cell class does not need to contain
// presentation-specific formatting logic.
internal static class ValueFormatter
{
    public static string Format<T>(
        T value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        // Dates are displayed consistently regardless
        // of how the DateTime was originally entered.
        if (value is DateTime dateTime)
        {
            return dateTime.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);
        }

        // Use invariant culture so decimal separators
        // do not depend on the computer's regional settings.
        if (value is double doubleValue)
        {
            return doubleValue.ToString(
                "G",
                CultureInfo.InvariantCulture);
        }

        if (value is decimal decimalValue)
        {
            return decimalValue.ToString(
                "G",
                CultureInfo.InvariantCulture);
        }

        // Booleans are displayed as simple lowercase text.
        if (value is bool boolValue)
        {
            return boolValue
                ? "true"
                : "false";
        }

        // Strings and other supported values use their
        // normal string representation.
        return value.ToString() ??
            string.Empty;
    }
}