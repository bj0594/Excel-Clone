using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExcelClone;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            ShowWelcome();

            TableSize tableSize = TableSetup.Create();

            if (tableSize.Cancelled)
            {
                Console.CursorVisible = true;
                return;
            }

            Table table = new Table(
                tableSize.DataRows,
                tableSize.Columns);

            TableEditor.Run(table);

            Console.Clear();
            Console.WriteLine("Table closed.");
            Console.WriteLine();
            Console.Write("Create another table? (Y/N): ");

            ConsoleKey key =
                Console.ReadKey(true).Key;

            if (key != ConsoleKey.Y)
            {
                Console.CursorVisible = true;
                return;
            }
        }
    }

    private static void ShowWelcome()
    {
        Console.WriteLine(
            "============================================");

        Console.WriteLine(
            "                 EXCEL CLONE");

        Console.WriteLine(
            "============================================");

        Console.WriteLine();

        Console.WriteLine(
            "A small terminal-based spreadsheet.");

        Console.WriteLine();

        Console.WriteLine(
            "Press any key to continue...");

        Console.ReadKey(true);
    }
}

internal readonly record struct TableSize(
    int DataRows,
    int Columns,
    bool Cancelled);

internal static class TableSetup
{
    private const int MinRows = 1;
    private const int MaxRows = 12;

    private const int MinColumns = 1;
    private const int MaxColumns = 8;

    public static TableSize Create()
    {
        int rows = 4;
        int columns = 3;

        // 0 = Columns
        // 1 = Rows
        int selected = 0;

        while (true)
        {
            Console.Clear();

            Console.WriteLine(
                "============================================");

            Console.WriteLine(
                "                 CREATE TABLE");

            Console.WriteLine(
                "============================================");

            Console.WriteLine();

            Console.WriteLine(
                "↑ / ↓  Select Columns or Rows");

            Console.WriteLine(
                "← / →  Change selected value");

            Console.WriteLine(
                "Enter   Create table");

            Console.WriteLine(
                "Esc     Quit");

            Console.WriteLine();

            DrawSelector(
                "Columns",
                columns,
                MinColumns,
                MaxColumns,
                selected == 0);

            DrawSelector(
                "Rows",
                rows,
                MinRows,
                MaxRows,
                selected == 1);

            Console.WriteLine();

            Console.WriteLine(
                $"Table: {columns} columns × {rows} data rows");

            ConsoleKey key =
                Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:

                    selected = 0;

                    break;

                case ConsoleKey.DownArrow:

                    selected = 1;

                    break;

                case ConsoleKey.LeftArrow:

                    if (selected == 0)
                    {
                        columns =
                            Math.Max(
                                columns - 1,
                                MinColumns);
                    }
                    else
                    {
                        rows =
                            Math.Max(
                                rows - 1,
                                MinRows);
                    }

                    break;

                case ConsoleKey.RightArrow:

                    if (selected == 0)
                    {
                        columns =
                            Math.Min(
                                columns + 1,
                                MaxColumns);
                    }
                    else
                    {
                        rows =
                            Math.Min(
                                rows + 1,
                                MaxRows);
                    }

                    break;

                case ConsoleKey.Enter:

                    return new TableSize(
                        rows,
                        columns,
                        false);

                case ConsoleKey.Escape:

                    return new TableSize(
                        0,
                        0,
                        true);
            }
        }
    }

    private static void DrawSelector(
        string label,
        int value,
        int minimum,
        int maximum,
        bool selected)
    {
        string marker =
            selected ? ">" : " ";

        Console.WriteLine(
            $"{marker} {label,-10}[ {value,2} ]   ({minimum}-{maximum})");
    }
}

internal static class TableEditor
{
    public static void Run(Table table)
    {
        int activeRow = 0;
        int activeColumn = 0;

        RenderTable(
            table,
            activeRow,
            activeColumn);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:

                    activeRow =
                        Math.Max(
                            activeRow - 1,
                            0);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.DownArrow:

                    activeRow =
                        Math.Min(
                            activeRow + 1,
                            table.TotalDisplayRows - 1);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.LeftArrow:

                    activeColumn =
                        Math.Max(
                            activeColumn - 1,
                            0);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.RightArrow:

                    activeColumn =
                        Math.Min(
                            activeColumn + 1,
                            table.ColumnCount - 1);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.Enter:

                    EditResult enterResult =
                        CellEditor.Edit(
                            table,
                            activeRow,
                            activeColumn,
                            null);

                    MoveAfterEditing(
                        table,
                        ref activeRow,
                        ref activeColumn,
                        enterResult);

                    RenderTable(
                        table,
                        activeRow,
                        activeColumn);

                    break;

                case ConsoleKey.Escape:

                    return;

                default:

                    if (!char.IsControl(
                            key.KeyChar))
                    {
                        EditResult typingResult =
                            CellEditor.Edit(
                                table,
                                activeRow,
                                activeColumn,
                                key.KeyChar);

                        MoveAfterEditing(
                            table,
                            ref activeRow,
                            ref activeColumn,
                            typingResult);

                        RenderTable(
                            table,
                            activeRow,
                            activeColumn);
                    }

                    break;
            }
        }
    }

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

    private static void RenderTable(
        Table table,
        int activeRow,
        int activeColumn)
    {
        Console.CursorVisible = false;

        Console.SetCursorPosition(
            0,
            0);

        TableRenderer.Render(
            table,
            activeRow,
            activeColumn);
    }
}

internal enum EditResult
{
    Stay,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight
}

internal static class CellEditor
{
    public static EditResult Edit(
        Table table,
        int displayRow,
        int column,
        char? firstCharacter)
    {
        bool isHeader =
            displayRow == 0;

        string originalValue =
            isHeader
                ? table.GetHeader(column)
                : table
                    .GetDataCell(
                        displayRow - 1,
                        column)
                    .DisplayValue;

        string buffer;

        if (firstCharacter.HasValue)
        {
            // Direct typing replaces the
            // current value.
            buffer =
                firstCharacter.Value
                    .ToString();
        }
        else
        {
            // Enter starts editing the
            // current value.
            buffer =
                originalValue;
        }

        RenderEditingState(
            table,
            displayRow,
            column,
            buffer);

        while (true)
        {
            ConsoleKeyInfo key =
                Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.Stay;

                case ConsoleKey.Escape:

                    return EditResult.Stay;

                case ConsoleKey.UpArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveUp;

                case ConsoleKey.DownArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveDown;

                case ConsoleKey.LeftArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveLeft;

                case ConsoleKey.RightArrow:

                    Commit(
                        table,
                        displayRow,
                        column,
                        buffer);

                    return EditResult.MoveRight;

                case ConsoleKey.Backspace:

                    if (buffer.Length > 0)
                    {
                        buffer =
                            buffer[..^1];

                        RenderEditingState(
                            table,
                            displayRow,
                            column,
                            buffer);
                    }

                    break;

                default:

                    if (!char.IsControl(
                            key.KeyChar))
                    {
                        buffer +=
                            key.KeyChar;

                        RenderEditingState(
                            table,
                            displayRow,
                            column,
                            buffer);
                    }

                    break;
            }
        }
    }

    private static void Commit(
        Table table,
        int displayRow,
        int column,
        string buffer)
    {
        Console.CursorVisible = false;

        if (displayRow == 0)
        {
            table.SetHeader(
                column,
                buffer);

            return;
        }

        ICell cell =
            CellFactory.Create(
                buffer);

        table.SetDataCell(
            displayRow - 1,
            column,
            cell);
    }

    private static void RenderEditingState(
        Table table,
        int displayRow,
        int column,
        string buffer)
    {
        Console.Clear();

        Console.SetCursorPosition(
            0,
            0);

        TableRenderer.Render(
            table,
            displayRow,
            column,
            buffer,
            true);

        Console.SetCursorPosition(
            CalculateInputCursorX(column),
            CalculateInputCursorY(displayRow));

        Console.CursorVisible = true;
    }

    private static int CalculateInputCursorX(
        int column)
    {
        const int cellWidth = 15;

        return
            column *
            (cellWidth + 3) +
            3;
    }

    private static int CalculateInputCursorY(
        int displayRow)
    {
        return
            displayRow * 2 + 1;
    }
}

internal sealed class Table
{
    private readonly List<Row> rows;
    private readonly List<string> headers;

    public int DataRowCount { get; }

    public int ColumnCount { get; }

    public int TotalDisplayRows =>
        DataRowCount + 1;

    public IReadOnlyList<Row> Rows =>
        rows;

    public IReadOnlyList<string> Headers =>
        headers;

    public Table(
        int dataRowCount,
        int columnCount)
    {
        if (dataRowCount < 1 ||
            dataRowCount > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataRowCount));
        }

        if (columnCount < 1 ||
            columnCount > 8)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columnCount));
        }

        DataRowCount =
            dataRowCount;

        ColumnCount =
            columnCount;

        headers =
            new List<string>(
                columnCount);

        for (int column = 0;
             column < columnCount;
             column++)
        {
            headers.Add(
                string.Empty);
        }

        rows =
            new List<Row>(
                dataRowCount);

        for (int row = 0;
             row < dataRowCount;
             row++)
        {
            rows.Add(
                new Row(
                    columnCount));
        }
    }

    public ICell GetDataCell(
        int dataRow,
        int column)
    {
        ValidateDataPosition(
            dataRow,
            column);

        return rows[dataRow]
            .GetCell(column);
    }

    public void SetDataCell(
        int dataRow,
        int column,
        ICell cell)
    {
        ValidateDataPosition(
            dataRow,
            column);

        rows[dataRow]
            .SetCell(
                column,
                cell);
    }

    public string GetHeader(
        int column)
    {
        ValidateColumn(column);

        return headers[column];
    }

    public void SetHeader(
        int column,
        string value)
    {
        ValidateColumn(column);

        headers[column] =
            value.Trim();
    }

    public TypeProfile AnalyzeColumn(
        int column)
    {
        ValidateColumn(column);

        IEnumerable<ICell> cells =
            rows.Select(
                row =>
                    row.GetCell(column));

        return TypeProfile.FromCells(
            cells);
    }

    private void ValidateColumn(
        int column)
    {
        if (column < 0 ||
            column >= ColumnCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }
    }

    private void ValidateDataPosition(
        int dataRow,
        int column)
    {
        ValidateColumn(column);

        if (dataRow < 0 ||
            dataRow >= DataRowCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataRow));
        }
    }
}

internal sealed class Row
{
    private readonly List<ICell> cells;

    public Row(
        int columnCount)
    {
        cells =
            new List<ICell>(
                columnCount);

        for (int column = 0;
             column < columnCount;
             column++)
        {
            cells.Add(
                CellFactory.Create(
                    string.Empty));
        }
    }

    public ICell GetCell(
        int column)
    {
        if (column < 0 ||
            column >= cells.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }

        return cells[column];
    }

    public void SetCell(
        int column,
        ICell cell)
    {
        if (column < 0 ||
            column >= cells.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(column));
        }

        cells[column] =
            cell;
    }
}

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

internal sealed class Cell<T> : ICell
    where T : IComparable<T>
{
    public T Value { get; }

    public Type ValueType =>
        typeof(T);

    public string TypeName =>
        typeof(T).Name;

    public object? RawValue =>
        Value;

    public string DisplayValue =>
        ValueFormatter.Format(
            Value);

    public bool IsEmpty =>
        Value is string text &&
        string.IsNullOrEmpty(text);

    public bool IsNumeric =>
        Value is int ||
        Value is double ||
        Value is decimal;

    public Cell(
        T value)
    {
        Value =
            value;
    }

    public bool TryGetDecimal(
        out decimal value)
    {
        if (Value is int intValue)
        {
            value =
                intValue;

            return true;
        }

        if (Value is double doubleValue)
        {
            value =
                (decimal)doubleValue;

            return true;
        }

        if (Value is decimal decimalValue)
        {
            value =
                decimalValue;

            return true;
        }

        value = 0;

        return false;
    }
}

internal static class CellFactory
{
    public static ICell Create(
        string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new Cell<string>(
                string.Empty);
        }

        if (int.TryParse(
                input,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int intValue))
        {
            return new Cell<int>(
                intValue);
        }

        if (double.TryParse(
                input,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double doubleValue))
        {
            return new Cell<double>(
                doubleValue);
        }

        if (bool.TryParse(
                input,
                out bool boolValue))
        {
            return new Cell<bool>(
                boolValue);
        }

        if (DateTime.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dateValue))
        {
            return new Cell<DateTime>(
                dateValue);
        }

        return new Cell<string>(
            input);
    }
}

internal static class ValueFormatter
{
    public static string Format<T>(
        T value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value is DateTime dateTime)
        {
            return dateTime.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);
        }

        if (value is double doubleValue)
        {
            return doubleValue.ToString(
                "G",
                CultureInfo.InvariantCulture);
        }

        if (value is decimal decimalValue)
        {
            return decimalValue.ToString(
                "G",
                CultureInfo.InvariantCulture);
        }

        if (value is bool boolValue)
        {
            return boolValue
                ? "true"
                : "false";
        }

        return value.ToString() ??
            string.Empty;
    }
}

internal enum DetectedType
{
    Empty,
    String,
    Int,
    Double,
    Bool,
    DateTime
}

internal sealed class TypeProfile
{
    public DetectedType DominantType
    {
        get;
        private init;
    }

    public int NonEmptyCount
    {
        get;
        private init;
    }

    public int StringCount
    {
        get;
        private init;
    }

    public int IntCount
    {
        get;
        private init;
    }

    public int DoubleCount
    {
        get;
        private init;
    }

    public int BoolCount
    {
        get;
        private init;
    }

    public int DateTimeCount
    {
        get;
        private init;
    }

    public bool IsNumeric =>
        IntCount + DoubleCount > 0;

    public static TypeProfile FromCells(
        IEnumerable<ICell> cells)
    {
        List<ICell> nonEmptyCells =
            cells
                .Where(
                    cell =>
                        !cell.IsEmpty)
                .ToList();

        if (nonEmptyCells.Count == 0)
        {
            return new TypeProfile
            {
                DominantType =
                    DetectedType.Empty,

                NonEmptyCount =
                    0
            };
        }

        int strings =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(string));

        int ints =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(int));

        int doubles =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(double));

        int bools =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(bool));

        int dates =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(DateTime));

        DetectedType dominantType =
            DetermineDominantType(
                strings,
                ints,
                doubles,
                bools,
                dates);

        return new TypeProfile
        {
            DominantType =
                dominantType,

            NonEmptyCount =
                nonEmptyCells.Count,

            StringCount =
                strings,

            IntCount =
                ints,

            DoubleCount =
                doubles,

            BoolCount =
                bools,

            DateTimeCount =
                dates
        };
    }

    private static DetectedType
        DetermineDominantType(
            int strings,
            int ints,
            int doubles,
            int bools,
            int dates)
    {
        Dictionary<DetectedType, int>
            counts =
                new()
                {
                    [DetectedType.String] =
                        strings,

                    [DetectedType.Int] =
                        ints,

                    [DetectedType.Double] =
                        doubles,

                    [DetectedType.Bool] =
                        bools,

                    [DetectedType.DateTime] =
                        dates
                };

        return counts
            .OrderByDescending(
                pair =>
                    pair.Value)
            .ThenBy(
                pair =>
                    TypePriority(
                        pair.Key))
            .First()
            .Key;
    }

    private static int TypePriority(
        DetectedType type)
    {
        return type switch
        {
            DetectedType.Int => 0,
            DetectedType.Double => 1,
            DetectedType.DateTime => 2,
            DetectedType.Bool => 3,
            DetectedType.String => 4,
            _ => 5
        };
    }
}

internal static class TableRenderer
{
    private const int CellWidth = 15;

    public static void Render(
        Table table,
        int activeRow,
        int activeColumn)
    {
        Render(
            table,
            activeRow,
            activeColumn,
            null,
            false);
    }

    public static void Render(
        Table table,
        int activeRow,
        int activeColumn,
        string? editingValue,
        bool editing)
    {
        Console.SetCursorPosition(
            0,
            0);

        Console.WriteLine(
            BuildTopBorder(
                table.ColumnCount));

        RenderHeader(
            table,
            activeRow,
            activeColumn,
            editingValue,
            editing);

        Console.WriteLine(
            BuildMiddleBorder(
                table.ColumnCount));

        for (int dataRow = 0;
             dataRow < table.DataRowCount;
             dataRow++)
        {
            int displayRow =
                dataRow + 1;

            for (int column = 0;
                 column < table.ColumnCount;
                 column++)
            {
                ICell cell =
                    table.GetDataCell(
                        dataRow,
                        column);

                bool active =
                    activeRow ==
                        displayRow &&
                    activeColumn ==
                        column;

                string value =
                    active &&
                    editing &&
                    editingValue != null
                        ? editingValue
                        : cell.DisplayValue;

                RenderDataCell(
                    value,
                    active);
            }

            Console.WriteLine();

            if (dataRow <
                table.DataRowCount - 1)
            {
                Console.WriteLine(
                    BuildMiddleBorder(
                        table.ColumnCount));
            }
        }

        Console.WriteLine(
            BuildBottomBorder(
                table.ColumnCount));
    }

    private static void RenderHeader(
        Table table,
        int activeRow,
        int activeColumn,
        string? editingValue,
        bool editing)
    {
        for (int column = 0;
             column < table.ColumnCount;
             column++)
        {
            bool active =
                activeRow == 0 &&
                activeColumn == column;

            string header =
                table.GetHeader(column);

            bool hasHeader =
                !string.IsNullOrEmpty(
                    header);

            if (active &&
                editing &&
                editingValue != null)
            {
                header =
                    editingValue;

                hasHeader =
                    !string.IsNullOrEmpty(
                        header);
            }

            if (!hasHeader &&
                !(active && editing))
            {
                header =
                    $"Header {column + 1}";
            }

            RenderHeaderCell(
                header,
                active,
                !hasHeader &&
                !(active && editing));
        }

        Console.WriteLine("│");
    }

    private static void RenderHeaderCell(
        string value,
        bool active,
        bool placeholder)
    {
        string content =
            Fit(
                value,
                CellWidth);

        if (active)
        {
            content =
                "> " + content;

            content =
                Fit(
                    content,
                    CellWidth);
        }

        if (placeholder &&
            !active)
        {
            Console.ForegroundColor =
                ConsoleColor.DarkGray;
        }

        Console.Write(
            $"│ {content.PadRight(CellWidth)} ");

        Console.ResetColor();
    }

    private static void RenderDataCell(
        string value,
        bool active)
    {
        string content =
            Fit(
                value,
                CellWidth);

        if (active)
        {
            content =
                "> " + content;

            content =
                Fit(
                    content,
                    CellWidth);
        }

        Console.Write(
            $"│ {content.PadRight(CellWidth)} ");
    }

    private static string BuildTopBorder(
        int columnCount)
    {
        string cellBorder =
            new string(
                '─',
                CellWidth + 2);

        return
            "┌" +
            string.Join(
                "┬",
                Enumerable.Repeat(
                    cellBorder,
                    columnCount)) +
            "┐";
    }

    private static string BuildMiddleBorder(
        int columnCount)
    {
        string cellBorder =
            new string(
                '─',
                CellWidth + 2);

        return
            "├" +
            string.Join(
                "┼",
                Enumerable.Repeat(
                    cellBorder,
                    columnCount)) +
            "┤";
    }

    private static string BuildBottomBorder(
        int columnCount)
    {
        string cellBorder =
            new string(
                '─',
                CellWidth + 2);

        return
            "└" +
            string.Join(
                "┴",
                Enumerable.Repeat(
                    cellBorder,
                    columnCount)) +
            "┘";
    }

    private static string Fit(
        string value,
        int width)
    {
        if (value.Length <= width)
        {
            return value;
        }

        if (width <= 3)
        {
            return value[..width];
        }

        return value[..(width - 3)] +
            "...";
    }
}