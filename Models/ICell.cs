using System;

namespace ExcelClone.Models;

// Common contract for all spreadsheet cells.
//
// Cell<T> is generic, but the rest of the application does
// not need to know which T a particular cell contains.
// This allows different Cell<T> types to coexist in a table.
internal interface ICell
{
    // Provides access to the underlying value without
    // exposing the cell's generic type parameter.
    object? RawValue
    {
        get;
    }

    // Identifies the concrete type stored by the cell.
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

    // Indicates whether the cell represents an empty value.
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

    // Provides a common way for numeric cells to expose
    // their value to calculations such as Sum.
    bool TryGetDecimal(
        out decimal value);
}