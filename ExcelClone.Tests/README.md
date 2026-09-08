# Excel Clone – Tests

This project contains unit tests for the generic `Container<T>` class from the main Excel Clone project.

The tests are written with xUnit and follow the Arrange–Act–Assert pattern. They cover both normal behaviour and relevant edge cases.

## What Is Tested?

The test suite covers:

- `Add()` and correct updates to `Count`
- `Get()` and retrieving values by index
- `Remove()` and its effect on the container
- `Clear()` on populated and empty containers
- Duplicate values
- Removing values that do not exist
- Invalid indexes and `ArgumentOutOfRangeException`
- `int`, `string`, and nullable string values
- Using `Container<T>` through the `IContainer<T>` interface
- Data-driven tests using `[Theory]` and `[InlineData]`

The tests focus on the public behaviour of the container rather than its private implementation details.

## Why These Tests?

`Container<T>` uses `List<T>` internally, but the internal collection is not exposed directly. The tests therefore verify the public behaviour of the class instead of testing the underlying `List<T>` implementation.

Several different values and generic types are tested to show that the same implementation can be reused with different `T` values.

The interface is also tested directly to verify that `Container<T>` behaves correctly when accessed through `IContainer<T>` rather than through the concrete class.

The edge-case tests document the expected behaviour for empty containers, duplicate values, missing values, nullable values, and invalid indexes.

## Arrange–Act–Assert

The tests are structured using:

- Arrange: create and prepare the test data
- Act: perform the operation being tested
- Assert: verify the expected result

This keeps each test focused and makes the expected behaviour easy to understand.

## Running the Tests

From the solution root, run:

```bash
dotnet test
```

All tests are independent and deterministic. They do not depend on a specific execution order, external files, network access, or other external state.

## Example Usage

The tests also serve as examples of how `Container<T>` is intended to be used:

```csharp
Container<int> numbers =
    new Container<int>();

numbers.Add(10);
numbers.Add(20);

int value =
    numbers.Get(0);
```

The same implementation can be used with another type:

```csharp
Container<string> names =
    new Container<string>();
```

It can also be accessed through the generic interface:

```csharp
IContainer<int> numbers =
    new Container<int>();
```

This demonstrates how one generic implementation can work with different data types while the interface provides a common contract.

## Relation to the Main Project

`Container<T>` and `IContainer<T>` were created as part of the first assignment to explore generic classes, generic collections, and interfaces.

This test project builds on that work by verifying that the implementation behaves as expected. The tests therefore provide both quality assurance and practical examples of how the generic class and interface can be used.