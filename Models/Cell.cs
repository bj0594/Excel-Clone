using ExcelClone.Data;

namespace ExcelClone.Models;

// Generic spreadsheet cell.
//
// T represents the actual type stored in the cell.
// The constraint requires T to support comparison,
// which makes the generic class suitable for values
// that may need to be compared or sorted.
internal sealed class Cell<T> : ICell
    where T : IComparable<T>
{
    // The strongly typed value stored by this cell.
    public T Value { get; }

    // The actual CLR type represented by T.
    public Type ValueType =>
        typeof(T);

    // Human-readable name of the stored type.
    public string TypeName =>
        typeof(T).Name;

    // Provides access to the underlying value through
    // the non-generic ICell interface.
    public object? RawValue =>
        Value;

    // Converts the strongly typed value into the text
    // representation used by the terminal renderer.
    public string DisplayValue =>
        ValueFormatter.Format(
            Value);

    // A string containing no characters represents an
    // empty cell. Other values are considered non-empty.
    public bool IsEmpty =>
        Value is string text &&
        string.IsNullOrEmpty(text);

    // Indicates whether the stored value is one of the
    // numeric types supported by the spreadsheet.
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

    // Attempts to convert supported numeric values to
    // decimal so that numeric operations can use one
    // common representation.
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