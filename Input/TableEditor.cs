using ExcelClone.Models;
using ExcelClone.Rendering;

namespace ExcelClone.Input;

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

        FocusArea focus =
            FocusArea.Table;

        RenderTable(
            table,
            activeRow,
            activeColumn,
            focus,
            activeOperation);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                // ========================================
                // UP
                // ========================================

                case ConsoleKey.UpArrow:

                    if (focus ==
                        FocusArea.Table)
                    {
                        activeRow =
                            Math.Max(
                                activeRow - 1,
                                0);
                    }
                    else
                    {
                        if (activeOperation > 0)
                        {
                            activeOperation--;
                        }
                        else
                        {
                            // Leave the operation block
                            // and return to the last data row.
                            focus =
                                FocusArea.Table;

                            activeRow =
                                table.TotalDisplayRows - 1;
                        }
                    }

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn,
                        focus,
                        activeOperation);

                    break;

                // ========================================
                // DOWN
                // ========================================

                case ConsoleKey.DownArrow:

                    if (focus ==
                        FocusArea.Table)
                    {
                        if (activeRow <
                            table.TotalDisplayRows - 1)
                        {
                            activeRow++;
                        }
                        else if (HasOperations(
                            table,
                            activeColumn))
                        {
                            // Enter operation block.
                            focus =
                                FocusArea.Operations;

                            activeOperation = 0;
                        }
                    }
                    else
                    {
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

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn,
                        focus,
                        activeOperation);

                    break;

                // ========================================
                // LEFT
                // ========================================

                case ConsoleKey.LeftArrow:

                    if (focus ==
                        FocusArea.Operations)
                    {
                        MoveOperationColumn(
                            table,
                            ref activeColumn,
                            ref activeOperation,
                            -1);
                    }
                    else
                    {
                        activeColumn =
                            Math.Max(
                                activeColumn - 1,
                                0);

                        HandleColumnChange(
                            table,
                            ref focus,
                            ref activeOperation,
                            activeColumn);
                    }

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn,
                        focus,
                        activeOperation);

                    break;

                // ========================================
                // RIGHT
                // ========================================

                case ConsoleKey.RightArrow:

                    if (focus ==
                        FocusArea.Operations)
                    {
                        MoveOperationColumn(
                            table,
                            ref activeColumn,
                            ref activeOperation,
                            1);
                    }
                    else
                    {
                        activeColumn =
                            Math.Min(
                                activeColumn + 1,
                                table.ColumnCount - 1);

                        HandleColumnChange(
                            table,
                            ref focus,
                            ref activeOperation,
                            activeColumn);
                    }

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn,
                        focus,
                        activeOperation);

                    break;

                // ========================================
                // ENTER
                // ========================================

                case ConsoleKey.Enter:

                    if (focus ==
                        FocusArea.Table)
                    {
                        EditResult result =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                null);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            result);

                        HandleColumnChange(
                            table,
                            ref focus,
                            ref activeOperation,
                            activeColumn);

                        RenderTable(
                            table,
                            activeRow,
                            activeColumn,
                            focus,
                            activeOperation);
                    }
                    else
                    {
                        // Operation execution will be
                        // implemented later.
                    }

                    break;

                // ========================================
                // ESCAPE
                // ========================================

                case ConsoleKey.Escape:

                    return;

                // ========================================
                // DIRECT TYPING
                // ========================================

                default:

                    if (focus ==
                        FocusArea.Table &&
                        !char.IsControl(
                            key.KeyChar))
                    {
                        EditResult result =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                key.KeyChar);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            result);

                        HandleColumnChange(
                            table,
                            ref focus,
                            ref activeOperation,
                            activeColumn);

                        RenderTable(
                            table,
                            activeRow,
                            activeColumn,
                            focus,
                            activeOperation);
                    }

                    break;
            }
        }
    }

    // ============================================
    // OPERATION COLUMN MOVEMENT
    // ============================================

    private static void MoveOperationColumn(
        Table table,
        ref int activeColumn,
        ref int activeOperation,
        int direction)
    {
        int targetColumn =
            activeColumn + direction;

        // Stay in the current operation block
        // if the target is outside the table.
        if (targetColumn < 0 ||
            targetColumn >= table.ColumnCount)
        {
            return;
        }

        // Do not leave the operation block just
        // because the neighbouring column has
        // no detected type / operations.
        if (!HasOperations(
                table,
                targetColumn))
        {
            return;
        }

        activeColumn =
            targetColumn;

        int operationCount =
            GetOperationCount(
                table,
                activeColumn);

        if (operationCount <= 0)
        {
            return;
        }

        // Keep the same vertical position when
        // possible. If the new column has fewer
        // operations, move to its last valid one.
        activeOperation =
            Math.Min(
                activeOperation,
                operationCount - 1);
    }

    // ============================================
    // COLUMN / OPERATION FOCUS
    // ============================================

    private static void HandleColumnChange(
        Table table,
        ref FocusArea focus,
        ref int activeOperation,
        int activeColumn)
    {
        if (!HasOperations(
                table,
                activeColumn))
        {
            // The new column has no detected type.
            // Operation focus is therefore impossible.
            focus =
                FocusArea.Table;

            activeOperation = 0;

            return;
        }

        int operationCount =
            GetOperationCount(
                table,
                activeColumn);

        if (operationCount <= 0)
        {
            focus =
                FocusArea.Table;

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

    // ============================================
    // OPERATION COUNT
    // ============================================

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

    private static void RenderTable(
        Table table,
        int activeRow,
        int activeColumn,
        FocusArea focus,
        int activeOperation)
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
            activeOperation);
    }
}