using ExcelClone.Models;
using ExcelClone.Operations;
using ExcelClone.Rendering;

namespace ExcelClone.Input;

// Handles keyboard input, navigation, editing and operation selection.
internal static class TableEditor
{
    private enum FocusArea
    {
        Table,
        Operations
    }

    public static void Run(Table table)
    {
        int activeRow = 0;
        int activeColumn = 0;
        int activeOperation = 0;

        string? operationResult = null;

        FocusArea focus =
            FocusArea.Table;

        RenderCurrentState(
            table,
            activeRow,
            activeColumn,
            focus,
            activeOperation,
            operationResult);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:

                    operationResult = null;

                    HandleUp(
                        table,
                        ref activeRow,
                        ref activeOperation,
                        ref focus);

                    break;

                case ConsoleKey.DownArrow:

                    operationResult = null;

                    HandleDown(
                        table,
                        ref activeRow,
                        ref activeOperation,
                        activeColumn,
                        ref focus);

                    break;

                case ConsoleKey.LeftArrow:

                    operationResult = null;

                    HandleLeft(
                        table,
                        ref activeColumn,
                        ref activeOperation,
                        focus);

                    break;

                case ConsoleKey.RightArrow:

                    operationResult = null;

                    HandleRight(
                        table,
                        ref activeColumn,
                        ref activeOperation,
                        focus);

                    break;

                case ConsoleKey.Enter:

                    if (focus ==
                        FocusArea.Table)
                    {
                        operationResult = null;

                        EditResult editResult =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                null);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            editResult);

                        HandleOperationSelection(
                            table,
                            ref activeOperation,
                            activeColumn);
                    }
                    else
                    {
                        operationResult =
                            ExecuteOperation(
                                table,
                                activeColumn,
                                activeOperation);
                    }

                    break;

                case ConsoleKey.Backspace:

                    operationResult = null;

                    if (focus ==
                        FocusArea.Table)
                    {
                        EditResult editResult =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                null,
                                true);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            editResult);

                        HandleOperationSelection(
                            table,
                            ref activeOperation,
                            activeColumn);
                    }

                    break;

                case ConsoleKey.Escape:

                    return;

                default:

                    operationResult = null;

                    if (focus ==
                        FocusArea.Table &&
                        !char.IsControl(
                            key.KeyChar))
                    {
                        EditResult editResult =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                key.KeyChar);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            editResult);

                        HandleOperationSelection(
                            table,
                            ref activeOperation,
                            activeColumn);
                    }

                    break;
            }

            RenderCurrentState(
                table,
                activeRow,
                activeColumn,
                focus,
                activeOperation,
                operationResult);
        }
    }

    // ============================================
    // KEY HANDLING
    // ============================================

    private static void HandleUp(
        Table table,
        ref int activeRow,
        ref int activeOperation,
        ref FocusArea focus)
    {
        if (focus ==
            FocusArea.Table)
        {
            activeRow =
                Math.Max(
                    activeRow - 1,
                    0);

            return;
        }

        if (activeOperation > 0)
        {
            activeOperation--;

            return;
        }

        focus =
            FocusArea.Table;

        activeRow =
            table.TotalDisplayRows - 1;
    }

    private static void HandleDown(
        Table table,
        ref int activeRow,
        ref int activeOperation,
        int activeColumn,
        ref FocusArea focus)
    {
        if (focus ==
            FocusArea.Table)
        {
            if (activeRow <
                table.TotalDisplayRows - 1)
            {
                activeRow++;

                return;
            }

            if (HasOperations(
                    table,
                    activeColumn))
            {
                focus =
                    FocusArea.Operations;

                activeOperation = 0;
            }

            return;
        }

        int operationCount =
            GetOperationCount(
                table,
                activeColumn);

        if (activeOperation <
            operationCount - 1)
        {
            activeOperation++;
        }
    }

    private static void HandleLeft(
        Table table,
        ref int activeColumn,
        ref int activeOperation,
        FocusArea focus)
    {
        if (focus ==
            FocusArea.Table)
        {
            activeColumn =
                Math.Max(
                    activeColumn - 1,
                    0);

            HandleOperationSelection(
                table,
                ref activeOperation,
                activeColumn);

            return;
        }

        int targetColumn =
            FindPreviousOperationColumn(
                table,
                activeColumn);

        if (targetColumn ==
            activeColumn)
        {
            return;
        }

        activeColumn =
            targetColumn;

        HandleOperationSelection(
            table,
            ref activeOperation,
            activeColumn);
    }

    private static void HandleRight(
        Table table,
        ref int activeColumn,
        ref int activeOperation,
        FocusArea focus)
    {
        if (focus ==
            FocusArea.Table)
        {
            activeColumn =
                Math.Min(
                    activeColumn + 1,
                    table.ColumnCount - 1);

            HandleOperationSelection(
                table,
                ref activeOperation,
                activeColumn);

            return;
        }

        int targetColumn =
            FindNextOperationColumn(
                table,
                activeColumn);

        if (targetColumn ==
            activeColumn)
        {
            return;
        }

        activeColumn =
            targetColumn;

        HandleOperationSelection(
            table,
            ref activeOperation,
            activeColumn);
    }

    // ============================================
    // OPERATION EXECUTION
    // ============================================

    private static string? ExecuteOperation(
        Table table,
        int column,
        int operation)
    {
        if (OperationExecutor.TryExecute(
                table,
                column,
                operation,
                out string result))
        {
            return result;
        }

        return null;
    }

    // ============================================
    // OPERATION NAVIGATION
    // ============================================

    private static int FindPreviousOperationColumn(
        Table table,
        int currentColumn)
    {
        for (int column = currentColumn - 1;
             column >= 0;
             column--)
        {
            if (HasOperations(
                table,
                column))
            {
                return column;
            }
        }

        return currentColumn;
    }

    private static int FindNextOperationColumn(
        Table table,
        int currentColumn)
    {
        for (int column = currentColumn + 1;
             column < table.ColumnCount;
             column++)
        {
            if (HasOperations(
                table,
                column))
            {
                return column;
            }
        }

        return currentColumn;
    }

    private static void HandleOperationSelection(
        Table table,
        ref int activeOperation,
        int activeColumn)
    {
        int operationCount =
            GetOperationCount(
                table,
                activeColumn);

        if (operationCount <= 0)
        {
            activeOperation = 0;

            return;
        }

        activeOperation =
            Math.Min(
                activeOperation,
                operationCount - 1);
    }

    private static bool HasOperations(
        Table table,
        int column)
    {
        TypeProfile profile =
            table.AnalyzeColumn(
                column);

        return
            profile.DominantType !=
            DetectedType.Empty;
    }

    private static int GetOperationCount(
        Table table,
        int column)
    {
        TypeProfile profile =
            table.AnalyzeColumn(
                column);

        if (profile.DominantType ==
            DetectedType.Empty)
        {
            return 0;
        }

        if (profile.IsNumeric)
        {
            return 3;
        }

        return profile.DominantType switch
        {
            DetectedType.DateTime => 2,
            DetectedType.Bool => 2,
            DetectedType.String => 2,
            _ => 0
        };
    }

    // ============================================
    // EDITING MOVEMENT
    // ============================================

    private static void MoveAfterEditing(
        Table table,
        ref int activeRow,
        ref int activeColumn,
        EditResult result)
    {
        switch (result)
        {
            case EditResult.MoveUp:

                activeRow =
                    Math.Max(
                        activeRow - 1,
                        0);

                break;

            case EditResult.MoveDown:

                activeRow =
                    Math.Min(
                        activeRow + 1,
                        table.TotalDisplayRows - 1);

                break;

            case EditResult.MoveLeft:

                activeColumn =
                    Math.Max(
                        activeColumn - 1,
                        0);

                break;

            case EditResult.MoveRight:

                activeColumn =
                    Math.Min(
                        activeColumn + 1,
                        table.ColumnCount - 1);

                break;
        }
    }

    // ============================================
    // RENDER
    // ============================================

    // Renders the current editor state after each key event.
    private static void RenderCurrentState(
        Table table,
        int activeRow,
        int activeColumn,
        FocusArea focus,
        int activeOperation,
        string? operationResult)
    {
        Console.CursorVisible = false;

        Console.SetCursorPosition(
            0,
            0);

        TableRenderer.Render(
            table,
            activeRow,
            activeColumn,
            null,
            false,
            focus == FocusArea.Operations,
            activeOperation,
            operationResult);
    }
}