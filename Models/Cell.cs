using ExcelClone.Data;

namespace ExcelClone.Models;

// Generic spreadsheet cell.
//
// T represents the type of value stored in the cell.
// The constraint ensures that T supports comparison,
// which is useful when working with ordered values.
internal sealed class Cell<T> : ICell
    where T : IComparable<T>
{
    // The strongly typed value stored by the cell.
    public T Value { get; }

    // Exposes the concrete generic type through ICell.
    public Type ValueType =>
        typeof(T);

    // Provides the stored type as a readable name.
    public string TypeName =>
        typeof(T).Name;

    // Makes the underlying value available through
    // the non-generic ICell interface.
    public object? RawValue =>
        Value;

    // Converts the typed value into the representation
    // used by the terminal renderer.
    public string DisplayValue =>
        ValueFormatter.Format(
            Value);

    // An empty string represents an empty spreadsheet cell.
    public bool IsEmpty =>
        Value is string text &&
        string.IsNullOrEmpty(text);

    // These are the numeric types currently supported
    // by CellFactory and the spreadsheet operations.
    public bool IsNumeric =>
        Value is int ||
        Value is double;

    public Cell(
        T value)
    {
        Value =
            value;
    }

    // Converts supported numeric values to decimal so
    // calculations can use one common numeric representation.
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

        value = 0;

        return false;
    }
}