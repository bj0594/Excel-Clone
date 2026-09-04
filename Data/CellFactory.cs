using System;
using System.Globalization;
using ExcelClone.Models;

namespace ExcelClone.Data;

internal static class CellFactory
{
    private static readonly string[] DateFormats =
    [
        "dd.MM.yyyy",
        "d.M.yyyy",
        "dd.MM.yy",
        "d.M.yy"
    ];

    public static ICell Create(
        string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new Cell<string>(
                string.Empty);
        }

        string trimmedInput =
            input.Trim();

        if (int.TryParse(
                trimmedInput,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int intValue))
        {
            return new Cell<int>(
                intValue);
        }

        if (double.TryParse(
                trimmedInput,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double doubleValue))
        {
            return new Cell<double>(
                doubleValue);
        }

        if (bool.TryParse(
                trimmedInput,
                out bool boolValue))
        {
            return new Cell<bool>(
                boolValue);
        }

        if (DateTime.TryParseExact(
                trimmedInput,
                DateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime exactDate))
        {
            return new Cell<DateTime>(
                exactDate);
        }

        /*
         * Fallback for other date representations
         * that .NET can recognize.
         */
        if (DateTime.TryParse(
                trimmedInput,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime dateValue))
        {
            return new Cell<DateTime>(
                dateValue);
        }

        return new Cell<string>(
            trimmedInput);
    }
}