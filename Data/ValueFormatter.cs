using System.Globalization;

namespace ExcelClone.Data;

// Converts typed cell values into the text representation
// displayed by the terminal renderer.
internal static class ValueFormatter
{
    public static string Format<T>(
        T value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        // Dates use one consistent display format regardless
        // of how the value was entered.
        if (value is DateTime dateTime)
        {
            return dateTime.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);
        }

        // Invariant culture keeps numeric formatting
        // independent of the computer's regional settings.
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

        if (value is bool boolValue)
        {
            return boolValue
                ? "true"
                : "false";
        }

        return value.ToString() ??
            string.Empty;
    }
}