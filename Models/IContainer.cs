namespace ExcelClone.Models;

// Defines the contract for a generic container.
//
// Implementations are responsible for storing values of type T
// and providing basic collection operations.
internal interface IContainer<T>
{
    // Number of values currently stored.
    int Count
    {
        get;
    }

    // Adds a value to the end of the container.
    void Add(
        T item);

    // Retrieves the value at the specified index.
    //
    // Implementations should throw ArgumentOutOfRangeException
    // when the index is outside the valid range.
    T Get(
        int index);

    // Removes the first matching value, if present.
    //
    // Returns true when a value was removed and false when
    // no matching value was found.
    bool Remove(
        T item);

    // Removes all values from the container.
    void Clear();
}