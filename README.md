# Excel Clone

A terminal-based spreadsheet application written in C#.

The application allows the user to create a small table, enter data directly into cells, automatically detect data types, sort data, and perform basic calculations.

The project is designed to demonstrate object-oriented programming, generics, interfaces, collections, input handling, type detection, and basic data analysis.

## Features

- Create a table with a custom number of rows and columns.
- Navigate the table using the arrow keys.
- Edit cells directly by typing.
- Edit an existing cell with `Enter`.
- Start editing and remove the last character with `Backspace`.
- Cancel editing with `Escape`.
- Automatically detect the type of entered data.
- Supported data types:
  - `string`
  - `int`
  - `double`
  - `bool`
  - `DateTime`
- Headers are stored separately from data cells.
- Columns analyze which data type occurs most frequently.
- Empty cells are supported.
- Sort entire rows based on the selected column.
- Perform basic numeric calculations using `Sum`.
- Available operations depend on the detected column type.
- The table is rendered directly in the terminal.

## How to Use

Run the application.

The first screen allows you to choose the size of the table.

### Creating a Table

Use:

- `↑` / `↓` to switch between Columns and Rows.
- `←` / `→` to increase or decrease the selected value.
- `Enter` to create the table.
- `Esc` to exit.

The application limits the table size so that it remains practical to use in a terminal.

### Navigating the Table

Once the table has been created:

- `↑` moves up.
- `↓` moves down.
- `←` moves left.
- `→` moves right.
- `Enter` starts editing the current cell.
- Typing a character starts editing immediately.
- `Backspace` starts editing and removes the last character.
- `Escape` cancels editing.

The currently selected cell is indicated by `>`.

### Operation Block

When the cursor reaches the bottom of a column with a detected data type, an operation block becomes available below the table.

The operation block is navigated separately from the main table.

Use:

- `↑` / `↓` to select an operation.
- `←` / `→` to move between columns that have available operations.
- `Enter` to execute the selected operation.
- `↑` from the first operation returns to the last data row.

Columns without a detected type cannot be selected in the operation block.

## Automatic Type Detection

Data entered into a cell is converted into an appropriate C# type when the value is committed.

The application currently recognizes:

```text
42              → int
42.5            → double
true            → bool
13.04.1965      → DateTime
Alice           → string
```

Dates can be entered using the format:

```text
dd.MM.yyyy
```

For example:

```text
13.04.1965
01.01.2020
07.08.2012
```

The value may be displayed using a different format in the table, but it is stored internally as a `DateTime`.

### Dominant Column Type

The type used by the operation block is determined from the values currently present in the column.

Each actual data type is considered separately:

```text
String
Int
Double
Bool
DateTime
```

The type with the highest number of occurrences becomes the dominant type.

If two or more types occur equally often, the type that appears first in the column wins the tie.

For example:

```text
10
20
Alice
```

results in `Int` because integers occur twice.

```text
Alice
10
Bob
```

results in `String` because strings occur twice.

```text
Alice
10
```

results in `String` because both types occur once and `String` appears first.

The dominant type is recalculated from the current contents of the column whenever the data changes.

## Operations

The available operations depend on the dominant type of the selected column.

### Numeric Columns

Numeric columns support:

```text
Low → High
High → Low
Sum
```

`Low → High` sorts the entire table from the smallest value to the largest value.

`High → Low` sorts the entire table from the largest value to the smallest value.

Sorting moves complete rows rather than only changing the selected column. This preserves the relationship between values in different columns.

Empty cells are always moved to the bottom of the sorted data.

`Sum` adds all numeric values in the selected column and displays the result in the operation block.

For example:

```text
10
20
30
```

produces:

```text
Sum: 60
```

### String Columns

String columns support:

```text
A → Z
Z → A
```

The entire table is sorted alphabetically based on the selected column.

### Date Columns

Date columns support:

```text
Old → New
New → Old
```

Dates are compared using their actual `DateTime` values rather than their displayed text.

### Boolean Columns

Boolean columns support:

```text
True first
False first
```

The entire table is sorted according to the selected boolean value.

## Generics

Generics are used in the project to create reusable code that can work with different data types without duplicating the implementation.

### Cell<T>

The main generic class in the spreadsheet is:

```csharp
internal sealed class Cell<T> : ICell
    where T : IComparable<T>
```

The generic type `T` represents the actual value stored inside a cell.

The same class can therefore represent different types:

```csharp
Cell<string>
Cell<int>
Cell<double>
Cell<bool>
Cell<DateTime>
```

The value is stored as:

```csharp
public T Value { get; }
```

This allows the cell implementation to remain generic while still working with strongly typed values.

### Container<T>

The project also contains a separate generic `Container<T>` class created to explore how generics can be combined with generic collections.

`Container<T>` stores its elements in:

```csharp
List<T>
```

It supports operations such as:

```csharp
Add()
Get()
Remove()
Clear()
```

The same container can therefore be used with different types without creating separate container classes for each type.

For example:

```csharp
Container<int>
Container<string>
```

use the same implementation while storing different kinds of values.

## Interfaces

The project also uses interfaces to define contracts between different parts of the application.

### ICell

`ICell` defines the common functionality that every cell must provide.

This allows the rest of the application to work with cells without needing to know the exact generic type used by `Cell<T>`.

For example, a `Table` can work with:

```csharp
ICell
```

regardless of whether the actual cell is:

```csharp
Cell<int>
Cell<string>
Cell<DateTime>
```

This separates common cell behavior from the specific implementation of the stored value.

### IContainer<T>

`Container<T>` implements:

```csharp
IContainer<T>
```

The interface defines the operations that a container must provide:

```csharp
Count
Add()
Get()
Remove()
Clear()
```

This means code using the container can depend on the interface instead of depending directly on the concrete `Container<T>` class.

This is useful in larger applications because an alternative implementation could replace `Container<T>` without changing the code that depends on the interface.

## Generic Constraints

`Cell<T>` uses a generic constraint:

```csharp
where T : IComparable<T>
```

This ensures that `T` supports comparison.

The constraint is useful because the application can rely on comparison behavior being available for the types used by `Cell<T>`.

Generic constraints therefore provide both flexibility and type safety.

## Why Generics and Interfaces Are Useful

The generic and interface-based design could be useful in a larger application.

For example, a spreadsheet system could eventually receive data from an API or database. A generic repository could store different entity types without requiring a separate repository implementation for every entity.

The same principles could also be used for:

- API models
- Database repositories
- Data validation
- Collections
- Sorting and comparison
- Game entities and components

Using interfaces would make it possible to replace implementations without changing the code that depends on their contracts.

## Project Structure

The project is organized into separate areas based on responsibility.

```text
ExcelClone/
│
├── Data/
│   ├── Cell.cs
│   ├── CellFactory.cs
│   └── ...
│
├── Input/
│   ├── TableEditor.cs
│   └── CellEditor.cs
│
├── Models/
│   ├── Table.cs
│   ├── Row.cs
│   ├── TypeProfile.cs
│   ├── Container.cs
│   ├── IContainer.cs
│   └── ...
│
├── Operations/
│   └── OperationsExecutor.cs
│
├── Rendering/
│   ├── TableRenderer.cs
│   └── OperationRenderer.cs
│
└── Program.cs
```

Each area has a separate responsibility:

- `Models` contains the main data structures and generic classes.
- `Data` contains cell-related implementations and factories.
- `Input` handles keyboard input and editing.
- `Operations` handles calculations and sorting.
- `Rendering` handles terminal output.
- `Program.cs` starts the application.

## Technologies

- C#
- .NET
- Console application
- Generics
- Interfaces
- Collections
- LINQ
- Object-oriented programming

## Assignment

This project was created as part of an assignment exploring generic types and generic classes in C#.

The project demonstrates:

- A custom generic class using `<T>`.
- Generic collections using `List<T>`.
- A generic constraint using `where T : IComparable<T>`.
- Interfaces and interface implementations.
- Multiple concrete types using the same generic implementation.

The spreadsheet functionality provides a practical context for demonstrating these concepts rather than keeping them as isolated examples.