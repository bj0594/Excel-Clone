using System;

namespace ExcelClone.Models;

internal interface IContainer<T>
{
    int Count { get; }

    void Add(
        T item);

    T Get(
        int index);

    bool Remove(
        T item);

    void Clear();
}