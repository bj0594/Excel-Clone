namespace ExcelClone.Models;

internal interface ICell
{
    Type ValueType { get; }

    string TypeName { get; }

    object? RawValue { get; }

    string DisplayValue { get; }

    bool IsEmpty { get; }

    bool IsNumeric { get; }

    bool TryGetDecimal(
        out decimal value);
}