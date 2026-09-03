namespace ExcelClone.Models;

internal readonly record struct TableSize(
    int DataRows,
    int Columns,
    bool Cancelled);