namespace ExcelClone.Input;

// Describes what should happen to the active cell
// after cell editing has finished.
internal enum EditResult
{
    Stay,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight
}