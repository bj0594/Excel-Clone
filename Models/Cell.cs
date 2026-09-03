using ExcelClone.Data;

namespace ExcelClone.Models;

internal sealed class Cell<T> : ICell
    where T : IComparable<T>
{
    public T Value { get; }

    public Type ValueType =>
        typeof(T);

    public string TypeName =>
        typeof(T).Name;

    public object? RawValue =>
        Value;

    public string DisplayValue =>
        ValueFormatter.Format(
            Value);

    public bool IsEmpty =>
        Value is string text &&
        string.IsNullOrEmpty(text);

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