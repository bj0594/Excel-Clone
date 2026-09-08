using System;
using ExcelClone.Models;
using Xunit;

namespace ExcelClone.Tests;

// Tests the generic Container<T> through both its concrete
// implementation and the IContainer<T> interface.
public class ContainerTests
{
    [Fact]
    public void Add_IncreasesCount_AndElementCanBeRetrieved()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        // Act
        container.Add(42);

        // Assert
        Assert.Equal(1, container.Count);
        Assert.Equal(42, container.Get(0));
    }

    [Fact]
    public void Get_ReturnsElementAtRequestedIndex()
    {
        // Arrange
        Container<string> container =
            new Container<string>();

        container.Add("A");
        container.Add("B");
        container.Add("C");

        // Act
        string result =
            container.Get(1);

        // Assert
        Assert.Equal("B", result);
    }

    [Fact]
    public void Remove_RemovesMatchingElement_AndDecreasesCount()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        container.Add(10);
        container.Add(20);
        container.Add(30);

        // Act
        bool removed =
            container.Remove(20);

        // Assert
        Assert.True(removed);
        Assert.Equal(2, container.Count);
        Assert.Equal(10, container.Get(0));
        Assert.Equal(30, container.Get(1));
    }

    [Fact]
    public void Remove_NonExistingValue_ReturnsFalse_AndKeepsContainerUnchanged()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        container.Add(10);
        container.Add(20);

        // Act
        bool removed =
            container.Remove(99);

        // Assert
        Assert.False(removed);
        Assert.Equal(2, container.Count);
        Assert.Equal(10, container.Get(0));
        Assert.Equal(20, container.Get(1));
    }

    [Fact]
    public void Remove_DuplicateValue_RemovesFirstMatchingElement()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        container.Add(10);
        container.Add(20);
        container.Add(20);

        // Act
        bool removed =
            container.Remove(20);

        // Assert
        Assert.True(removed);
        Assert.Equal(2, container.Count);
        Assert.Equal(10, container.Get(0));
        Assert.Equal(20, container.Get(1));
    }

    [Fact]
    public void Add_AllowsDuplicateValues()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        // Act
        container.Add(42);
        container.Add(42);

        // Assert
        Assert.Equal(2, container.Count);
        Assert.Equal(42, container.Get(0));
        Assert.Equal(42, container.Get(1));
    }

    [Fact]
    public void Clear_RemovesAllElements_AndResetsCount()
    {
        // Arrange
        Container<string> container =
            new Container<string>();

        container.Add("A");
        container.Add("B");

        // Act
        container.Clear();

        // Assert
        Assert.Equal(0, container.Count);
    }

    [Fact]
    public void Clear_OnEmptyContainer_LeavesCountAtZero()
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        // Act
        container.Clear();

        // Assert
        Assert.Equal(0, container.Count);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(999)]
    public void Get_InvalidIndex_ThrowsArgumentOutOfRangeException(
        int index)
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        container.Add(42);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => container.Get(index));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(-10)]
    public void Add_WithDifferentIntValues_StoresCorrectValue(
        int value)
    {
        // Arrange
        Container<int> container =
            new Container<int>();

        // Act
        container.Add(value);

        // Assert
        Assert.Equal(value, container.Get(0));
    }

    [Theory]
    [InlineData("alpha")]
    [InlineData("beta")]
    [InlineData("")]
    public void Add_WithDifferentStringValues_StoresCorrectValue(
        string value)
    {
        // Arrange
        Container<string> container =
            new Container<string>();

        // Act
        container.Add(value);

        // Assert
        Assert.Equal(value, container.Get(0));
    }

    [Fact]
    public void IContainer_CanBeUsedThroughGenericContract()
    {
        // Arrange
        IContainer<string> container =
            new Container<string>();

        // Act
        container.Add("Hello");

        // Assert
        Assert.Equal(1, container.Count);
        Assert.Equal("Hello", container.Get(0));
    }

    [Fact]
    public void Add_NullString_CanBeStoredAndRetrieved()
    {
        // Arrange
        Container<string?> container =
            new Container<string?>();

        // Act
        container.Add(null);

        // Assert
        Assert.Equal(1, container.Count);
        Assert.Null(container.Get(0));
    }
}