using System;
using System.Collections.Generic;

namespace ExcelClone.Models;

// A reusable generic container that stores values of any type in List<T>.
// The class implements IContainer<T>, demonstrating both generics and interfaces.
internal sealed class Container<T> : IContainer<T>
{
    private readonly List<T> items =
        new();

    public int Count =>
        items.Count;

    // Adds a value to the end of the container.
    public void Add(
        T item)
    {
        items.Add(
            item);
    }

    // Returns the value at the requested index.
    public T Get(
        int index)
    {
        if (index < 0 ||
            index >= items.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index));
        }

        return items[index];
    }

    // Removes the first matching value, if present.
    public bool Remove(
        T item)
    {
        return items.Remove(
            item);
    }

    // Removes all values from the container.
    public void Clear()
    {
        items.Clear();
    }
}