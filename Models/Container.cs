using System;
using System.Collections.Generic;

namespace ExcelClone.Models;

internal sealed class Container<T> : IContainer<T>
{
    private readonly List<T> items;

    public int Count =>
        items.Count;

    public Container()
    {
        items =
            new List<T>();
    }

    public void Add(
        T item)
    {
        items.Add(
            item);
    }

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

    public bool Remove(
        T item)
    {
        return items.Remove(
            item);
    }

    public void Clear()
    {
        items.Clear();
    }
}