using Xunit;
using ExcelClone.Models;

namespace ExcelClone.Tests;

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
}