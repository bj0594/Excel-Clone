namespace ExcelClone.Models;

// Contains the dimensions selected when creating a table,
// and whether the table setup was cancelled.
internal readonly record struct TableSize(
    int DataRows,
    int Columns,
    bool Cancelled);