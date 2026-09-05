using System;
using System.Collections.Generic;

namespace ExcelClone.Models;

// A reusable generic container that stores values of any type
// using List<T>. The class implements IContainer<T> and
// demonstrates generics together with interfaces.
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
        ValidateIndex(
            index);

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

    // Keeps index validation in one place.
    private void ValidateIndex(
        int index)
    {
        if (index < 0 ||
            index >= items.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index));
        }
    }
}