# Excel Clone – Tests

This project contains unit tests for the generic `Container<T>` class from the main project.

The tests use xUnit and mainly follow the Arrange–Act–Assert pattern. The goal is to verify both normal functionality and relevant edge cases.

## What Is Tested?

The test suite covers:

- `Add()` and correct updates to `Count`
- `Get()` and retrieving values by index
- `Remove()` and its effect on stored values and `Count`
- `Clear()` on both populated and empty containers
- Duplicate values and how `Remove()` handles them
- Invalid indexes and the expected `ArgumentOutOfRangeException`
- `int`, `string`, and `null` values
- Using `Container<T>` through the `IContainer<T>` interface
- Data-driven tests using `[Theory]` and `[InlineData]`

The tests focus on the public behaviour of the class rather than its private implementation details.

## Why These Tests?

`Container<T>` uses `List<T>` internally, but the internal collection is not exposed directly. The tests therefore verify the behaviour of the public methods instead of testing the `List<T>` implementation itself.

Several different types are tested to verify that the same generic class can work with different `T` values. The interface test verifies that `Container<T>` can also be used through the `IContainer<T>` contract.

The edge-case tests document the expected behaviour for empty containers, missing values, duplicate values, invalid indexes, and nullable reference values.

## Running the Tests

From the solution root, run:

```bash
dotnet test
```

Each test is independent and does not rely on a specific execution order or external state.

## Example Usage

The tests also provide a simple example of how `Container<T>` is intended to be used:

```csharp
Container<int> numbers =
    new Container<int>();

numbers.Add(10);
numbers.Add(20);

int value =
    numbers.Get(0);
```

The same implementation can be used with other types:

```csharp
Container<string> names =
    new Container<string>();
```

This demonstrates the same generic concept explored in the main project: one class can work with different data types without requiring a separate implementation for each type.

## Relation to the Main Project

`Container<T>` and `IContainer<T>` were created in the first part of the assignment to explore generic classes, generic collections, and interfaces.

This test project builds on that work by verifying that the implementation behaves as expected. The tests therefore serve both as quality assurance and as documentation of how the generic class can be used.