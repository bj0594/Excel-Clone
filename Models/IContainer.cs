namespace ExcelClone.Models;

// Defines the contract for a generic container.
// Any implementation of IContainer<T> must support
// adding, retrieving, removing, and clearing values.
internal interface IContainer<T>
{
    // Number of values currently stored.
    int Count { get; }

    // Adds a value to the container.
    void Add(
        T item);

    // Retrieves a value by index.
    T Get(
        int index);

    // Removes the first matching value.
    bool Remove(
        T item);

    // Removes all values from the container.
    void Clear();
}