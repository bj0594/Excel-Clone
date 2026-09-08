using System;

namespace ExcelClone.Models;

// Common contract for all spreadsheet cells.
//
// Cell<T> is generic, but the rest of the application
// can work with every cell through this interface without
// needing to know its concrete generic type.
internal interface ICell
{
    // Provides access to the underlying value.
    object? RawValue
    {
        get;
    }

    // Identifies the concrete type stored in the cell.
    Type ValueType
    {
        get;
    }

    // Provides the stored type as a readable name.
    string TypeName
    {
        get;
    }

    // Provides the value in the format used by the renderer.
    string DisplayValue
    {
        get;
    }

    // Indicates whether the cell contains no value.
    bool IsEmpty
    {
        get;
    }

    // Indicates whether the cell contains a supported
    // numeric value.
    bool IsNumeric
    {
        get;
    }

    // Provides a common numeric representation for
    // calculations such as Sum.
    bool TryGetDecimal(
        out decimal value);
}