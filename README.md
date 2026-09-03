# Excel Clone

A terminal-based spreadsheet application written in C#.

The application allows the user to create a small table, enter data directly into cells, and automatically detect the data type of each cell. The project is designed to demonstrate object-oriented programming, generics, interfaces, collections, input handling, and basic data analysis.

## Features

- Create a table with a custom number of rows and columns.
- Navigate the table using the arrow keys.
- Edit cells directly by typing.
- Edit an existing cell with `Enter`.
- Use `Backspace` while editing.
- Cancel editing with `Escape`.
- Automatically detect the type of entered data.
- Supported data types:
  - `string`
  - `int`
  - `double`
  - `bool`
  - `DateTime`
- Headers are stored separately from the data cells.
- Columns can analyze which data type is most common.
- Empty cells are supported.
- The table is rendered directly in the terminal.

## How to Use

Run the application.

The first screen allows you to choose the size of the table.

### Creating a table

Use:

- `↑` / `↓` to switch between Columns and Rows.
- `←` / `→` to increase or decrease the selected value.
- `Enter` to create the table.
- `Esc` to exit.

The application limits the table size so that it remains practical to use in a terminal.

### Navigating the table

Once the table has been created:

- `↑` moves up.
- `↓` moves down.
- `←` moves left.
- `→` moves right.
- `Enter` starts editing the current cell.
- Typing a character starts editing immediately.
- `Escape` cancels editing.

The currently selected cell is indicated by `>`.

## Automatic Type Detection

Data entered into a cell is automatically analyzed when the value is committed.

For example:

```text
42