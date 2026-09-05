using ExcelClone.Data;

namespace ExcelClone.Models;

// Generic spreadsheet cell.
//
// T determines the type of value stored by the cell.
// The IComparable<T> constraint allows supported values
// to be compared generically, which is useful for sorting.
internal sealed class Cell<T> : ICell
    where T : IComparable<T>
{
    // The strongly typed value stored by this cell.
    public T Value { get; }

    // Exposes the generic type through the non-generic interface.
    public Type ValueType =>
        typeof(T);

    // Provides the type name for display or inspection.
    public string TypeName =>
        typeof(T).Name;

    // Allows non-generic code to access the underlying value.
    public object? RawValue =>
        Value;

    // Converts the value into the text shown in the terminal.
    public string DisplayValue =>
        ValueFormatter.Format(
            Value);

    // Only an empty string represents an empty cell.
    public bool IsEmpty =>
        Value is string text &&
        string.IsNullOrEmpty(text);

    // Identifies the numeric types supported by the application.
    public bool IsNumeric =>
        Value is int ||
        Value is double ||
        Value is decimal;

    public Cell(
        T value)
    {
        Value =
            value;
    }

    // Converts supported numeric types to decimal so numeric
    // operations can use one common representation.
    public bool TryGetDecimal(
        out decimal value)
    {
        if (Value is int intValue)
        {
            value =
                intValue;

            return true;
        }

        if (Value is double doubleValue)
        {
            value =
                (decimal)doubleValue;

            return true;
        }

        if (Value is decimal decimalValue)
        {
            value =
                decimalValue;

            return true;
        }

        value = 0;

        return false;
    }
}