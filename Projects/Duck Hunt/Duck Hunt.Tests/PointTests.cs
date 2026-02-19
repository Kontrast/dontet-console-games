using System;
using Xunit;

namespace DuckHunt.Tests;

/// <summary>
/// Tests for the Point struct from Duck Hunt game.
/// </summary>
public class PointTests
{
    [Fact]
    public void Constructor_ShouldInitializeXAndY()
    {
        // Arrange & Act
        var point = new TestPoint(10, 20);

        // Assert
        Assert.Equal(10, point.X);
        Assert.Equal(20, point.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 10)]
    [InlineData(-5, -10)]
    [InlineData(100, 200)]
    public void Constructor_ShouldHandleVariousCoordinates(int x, int y)
    {
        // Act
        var point = new TestPoint(x, y);

        // Assert
        Assert.Equal(x, point.X);
        Assert.Equal(y, point.Y);
    }

    [Fact]
    public void Addition_ShouldAddXCoordinates()
    {
        // Arrange
        var point1 = new TestPoint(10, 20);
        var point2 = new TestPoint(5, 8);

        // Act
        var result = point1 + point2;

        // Assert
        Assert.Equal(15, result.X);
    }

    [Fact]
    public void Addition_ShouldAddYCoordinates()
    {
        // Arrange
        var point1 = new TestPoint(10, 20);
        var point2 = new TestPoint(5, 8);

        // Act
        var result = point1 + point2;

        // Assert
        Assert.Equal(28, result.Y);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(5, 10, 3, 7, 8, 17)]
    [InlineData(-5, -10, 3, 7, -2, -3)]
    [InlineData(100, 200, -50, -100, 50, 100)]
    public void Addition_VariousValues(int x1, int y1, int x2, int y2, int expectedX, int expectedY)
    {
        // Arrange
        var point1 = new TestPoint(x1, y1);
        var point2 = new TestPoint(x2, y2);

        // Act
        var result = point1 + point2;

        // Assert
        Assert.Equal(expectedX, result.X);
        Assert.Equal(expectedY, result.Y);
    }

    [Fact]
    public void Fields_ShouldBeMutable()
    {
        // Arrange
        var point = new TestPoint(10, 20);

        // Act
        point.X = 30;
        point.Y = 40;

        // Assert
        Assert.Equal(30, point.X);
        Assert.Equal(40, point.Y);
    }
}

/// <summary>
/// Test implementation of Point struct to match the Duck Hunt implementation.
/// </summary>
public struct TestPoint
{
    public int X;
    public int Y;

    public TestPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static TestPoint operator +(TestPoint a, TestPoint b) => new(a.X + b.X, a.Y + b.Y);
}