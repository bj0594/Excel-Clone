namespace ExcelClone.Models;

// Represents the data types that the application can detect in a cell.
internal enum DetectedType
{
    Empty,
    String,
    Int,
    Double,
    Bool,
    DateTime
}