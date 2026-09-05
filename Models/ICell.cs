using System;

namespace ExcelClone.Models;

// Common interface for every spreadsheet cell.
//
// Cell<T> is generic, but the rest of the application
// can work with all cell types through this interface.
// This keeps the table independent from the concrete
// generic type stored in each cell.
internal interface ICell
{
    // The underlying value without exposing the generic
    // type parameter to callers using ICell.
    object? RawValue
    {
        get;
    }

    // The runtime type stored by the generic cell.
    //
    // TypeProfile uses this to determine whether a column
    // primarily contains strings, integers, doubles,
    // booleans or DateTime values.
    Type ValueType
    {
        get;
    }

    // Human-readable name of the stored type.
    string TypeName
    {
        get;
    }

    // Text representation used when rendering the cell.
    string DisplayValue
    {
        get;
    }

    // Indicates whether the cell contains no value.
    bool IsEmpty
    {
        get;
    }

    // Indicates whether the stored value is numeric.
    bool IsNumeric
    {
        get;
    }

    // Attempts to convert the value to decimal.
    //
    // Numeric operations such as Sum can therefore
    // handle int and double through one common method.
    bool TryGetDecimal(
        out decimal value);
}