# Excel Clone

A small terminal-based spreadsheet application written in C#.

The project was created as a practical way to explore generic classes, generic collections, interfaces, type detection, and object-oriented design.

## Features

- Create a table with a custom number of columns and rows
- Navigate with the arrow keys
- Edit cells and headers
- Use `Enter` to edit and `Backspace` to remove characters
- Automatically detect `string`, `int`, `double`, `bool`, and `DateTime`
- Determine the dominant type of each column
- Sort complete rows based on a column
- Calculate `Sum` for numeric columns
- Keep empty cells at the bottom when sorting
- Show different operations depending on the detected column type

## Planning and Design

The project was planned around separation of responsibilities. `Table` and `Row` handle the spreadsheet structure, `Cell<T>` handles strongly typed cell values, `ICell` provides a common cell contract, and separate namespaces handle input, operations, data handling, and rendering.

The project is organized as follows:

```text
ExcelClone/
├── Data/
│   ├── CellFactory.cs
│   └── ValueFormatter.cs
├── Input/
│   ├── CellEditor.cs
│   ├── EditResult.cs
│   ├── TableEditor.cs
│   └── TableSetup.cs
├── Models/
│   ├── Cell.cs
│   ├── Container.cs
│   ├── DetectedType.cs
│   ├── ICell.cs
│   ├── IContainer.cs
│   ├── Row.cs
│   ├── Table.cs
│   ├── TableSize.cs
│   └── TypeProfile.cs
├── Operations/
│   └── OperationExecutor.cs
├── Rendering/
│   ├── OperationRenderer.cs
│   └── TableRenderer.cs
└── Program.cs
```

The goal was to avoid putting all functionality into `Program.cs` and instead keep each part of the application focused on one responsibility.

## Generics

The main generic class is `Cell<T>`:

```csharp
internal sealed class Cell<T> : ICell
    where T : IComparable<T>
```

`T` represents the type of value stored in the cell. The same implementation can therefore be reused with:

```csharp
Cell<string>
Cell<int>
Cell<double>
Cell<bool>
Cell<DateTime>
```

The `where T : IComparable<T>` constraint demonstrates a generic constraint and ensures that the types used with `Cell<T>` support comparison.

The project also contains a separate generic `Container<T>`:

```csharp
internal sealed class Container<T> : IContainer<T>
```

`Container<T>` stores values using `List<T>` and supports `Add`, `Get`, `Remove`, and `Clear`.

This demonstrates how one generic class can work with different data types without creating separate container classes.

## Interfaces

`ICell` defines the common contract for spreadsheet cells. This allows the rest of the application to work with different `Cell<T>` instances through the same abstraction.

For example:

```csharp
ICell
```

can represent:

```csharp
Cell<int>
Cell<string>
Cell<DateTime>
```

`IContainer<T>` defines the contract implemented by `Container<T>`.

Interfaces separate expected behaviour from implementation and make alternative implementations possible without changing the code that depends on the contract.

## Type Detection

When a value is committed, `CellFactory` attempts to convert the input into a suitable C# type.

```text
42          → int
42.5        → double
true        → bool
13.04.1965  → DateTime
Alice       → string
```

Dates can be entered using:

```text
dd.MM.yyyy
```

The value may be displayed in another format, but it remains stored internally as a `DateTime`.

`TypeProfile` analyses the current values in a column. The type occurring most often becomes the dominant type used by the operation block.

For example:

```text
Alice
10
Bob
```

results in `String` because two values are strings.

## Operations

Available operations depend on the dominant type of the selected column.

Numeric:

```text
Low → High
High → Low
Sum
```

String:

```text
A → Z
Z → A
```

Date:

```text
Old → New
New → Old
```

Boolean:

```text
True first
False first
```

Sorting moves complete rows instead of individual cells, keeping values from different columns associated with the same row.

Empty cells are always placed at the bottom.

## Why This Approach

The spreadsheet provides a practical context for the generic and interface concepts from the assignment.

`Cell<T>` avoids creating separate cell classes for each supported data type, while `ICell` allows those different generic instances to be handled through one common contract.

`Container<T>` demonstrates how the same generic implementation can work with different types while using `List<T>` internally.

The same ideas could later be applied to API models, database repositories, validation systems, or other collections of strongly typed objects.

## Technologies

- C#
- .NET
- Console application
- Generics
- Generic collections
- Interfaces
- Object-oriented programming

## Assignment

This project was created as part of an assignment focused on generic types and generic classes.

It demonstrates:

- A custom generic class using `T`
- Generic collections using `List<T>`
- A generic constraint using `where T : IComparable<T>`
- Interfaces and implementations
- Reusing the same generic implementation with multiple data types

The spreadsheet functionality provides a practical context for these concepts instead of keeping the generic examples isolated from the rest of the application.