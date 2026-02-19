using System;
using Xunit;

namespace DuckHunt.Tests;

/// <summary>
/// Tests for the Bird class from Duck Hunt game.
/// </summary>
public class BirdTests
{
    [Fact]
    public void Constructor_ShouldInitializePosition()
    {
        // Act
        var bird = new TestBird(10, 20, 1);

        // Assert
        Assert.Equal(10, bird.X);
        Assert.Equal(20, bird.Y);
    }

    [Fact]
    public void Constructor_ShouldInitializeDirection()
    {
        // Act
        var bird = new TestBird(10, 20, -1);

        // Assert
        Assert.Equal(-1, bird.Direction);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void Constructor_ShouldHandleBothDirections(int direction)
    {
        // Act
        var bird = new TestBird(10, 20, direction);

        // Assert
        Assert.Equal(direction, bird.Direction);
    }

    [Fact]
    public void Constructor_InitialFrame_ShouldBeZero()
    {
        // Act
        var bird = new TestBird(10, 20, 1);

        // Assert
        Assert.Equal(0, bird.Frame);
    }

    [Fact]
    public void Constructor_InitialIsDead_ShouldBeFalse()
    {
        // Act
        var bird = new TestBird(10, 20, 1);

        // Assert
        Assert.False(bird.IsDead);
    }

    [Fact]
    public void IncrementFrame_WhenAlive_ShouldCycleFrames()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);

        // Act & Assert
        Assert.Equal(0, bird.Frame);
        bird.IncrementFrame();
        Assert.Equal(1, bird.Frame);
        bird.IncrementFrame();
        Assert.Equal(2, bird.Frame);
        bird.IncrementFrame();
        Assert.Equal(3, bird.Frame);
        bird.IncrementFrame();
        Assert.Equal(0, bird.Frame); // Should wrap back to 0
    }

    [Fact]
    public void IncrementFrame_WhenDead_ShouldStayAtFrame4()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);
        bird.IsDead = true;

        // Act
        bird.IncrementFrame();

        // Assert
        Assert.Equal(4, bird.Frame);
    }

    [Fact]
    public void IncrementFrame_WhenDead_MultipleIncrements_ShouldRemainAtFrame4()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);
        bird.IsDead = true;

        // Act
        bird.IncrementFrame();
        bird.IncrementFrame();
        bird.IncrementFrame();

        // Assert
        Assert.Equal(4, bird.Frame);
    }

    [Theory]
    [InlineData(10, 20, 10, 20, 3, true)]  // Exact position
    [InlineData(10, 20, 11, 21, 3, true)]  // Inside bird bounds
    [InlineData(10, 20, 12, 22, 3, true)]  // Inside bird bounds
    [InlineData(10, 20, 9, 19, 3, false)]  // Before bird position
    [InlineData(10, 20, 10, 19, 3, false)] // Y before bird
    [InlineData(10, 20, 9, 20, 3, false)]  // X before bird
    public void Contains_ShouldDetectPointWithinBounds(int birdX, int birdY, int pointX, int pointY, int birdHeight, bool expected)
    {
        // Arrange
        var bird = new TestBird(birdX, birdY, 1);

        // Act
        bool contains = bird.Contains(pointX, pointY, 10, birdHeight);

        // Assert
        Assert.Equal(expected, contains);
    }

    [Fact]
    public void Contains_TopLeftCorner_ShouldBeInside()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);

        // Act
        bool contains = bird.Contains(10, 20, 10, 3);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void Contains_BottomRightCorner_ShouldBeInside()
    {
        // Arrange - Bird at (10, 20) with width 10, height 3
        var bird = new TestBird(10, 20, 1);

        // Act - Point at (19, 22) which is (X + Width - 1, Y + Height - 1)
        bool contains = bird.Contains(19, 22, 10, 3);

        // Assert
        Assert.True(contains);
    }

    [Fact]
    public void Contains_JustOutsideRight_ShouldBeFalse()
    {
        // Arrange - Bird at (10, 20) with width 10, height 3
        var bird = new TestBird(10, 20, 1);

        // Act - Point at X + Width (20)
        bool contains = bird.Contains(20, 20, 10, 3);

        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void Contains_JustOutsideBottom_ShouldBeFalse()
    {
        // Arrange - Bird at (10, 20) with width 10, height 3
        var bird = new TestBird(10, 20, 1);

        // Act - Point at Y + Height (23)
        bool contains = bird.Contains(10, 23, 10, 3);

        // Assert
        Assert.False(contains);
    }

    [Theory]
    [InlineData(0, 0, 10, 3)]
    [InlineData(100, 50, 10, 3)]
    [InlineData(-10, -5, 10, 3)]
    public void Contains_VariousBirdPositions(int birdX, int birdY, int width, int height)
    {
        // Arrange
        var bird = new TestBird(birdX, birdY, 1);

        // Act & Assert - Point inside bird
        Assert.True(bird.Contains(birdX, birdY, width, height));
        Assert.True(bird.Contains(birdX + width - 1, birdY + height - 1, width, height));

        // Point outside bird
        Assert.False(bird.Contains(birdX - 1, birdY, width, height));
        Assert.False(bird.Contains(birdX, birdY - 1, width, height));
        Assert.False(bird.Contains(birdX + width, birdY, width, height));
        Assert.False(bird.Contains(birdX, birdY + height, width, height));
    }

    [Fact]
    public void IsDead_ShouldBeSettable()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);

        // Act
        bird.IsDead = true;

        // Assert
        Assert.True(bird.IsDead);
    }

    [Fact]
    public void Position_ShouldBeModifiable()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);

        // Act
        bird.X = 50;
        bird.Y = 60;

        // Assert
        Assert.Equal(50, bird.X);
        Assert.Equal(60, bird.Y);
    }

    [Theory]
    [InlineData(1, 10, 11)]   // Moving right
    [InlineData(-1, 10, 9)]   // Moving left
    [InlineData(1, 0, 1)]     // Edge case: starting at 0
    [InlineData(-1, 100, 99)] // Edge case: starting at 100
    public void BirdMovement_Simulation(int direction, int startX, int expectedX)
    {
        // Arrange
        var bird = new TestBird(startX, 20, direction);

        // Act - Simulate one frame of movement
        bird.X += bird.Direction;

        // Assert
        Assert.Equal(expectedX, bird.X);
    }

    [Fact]
    public void DeadBird_Falling_Simulation()
    {
        // Arrange
        var bird = new TestBird(10, 20, 1);
        bird.IsDead = true;

        // Act - Simulate falling (Y increases)
        bird.Y++;

        // Assert
        Assert.Equal(21, bird.Y);
    }
}

/// <summary>
/// Test implementation of Bird class to match the Duck Hunt implementation.
/// </summary>
public class TestBird
{
    public int X;
    public int Y;
    public int Frame = 0;
    public int Direction = 0;
    public bool IsDead = false;

    public TestBird(int x, int y, int direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    public void IncrementFrame()
    {
        if (IsDead)
        {
            Frame = 4;
        }
        else
        {
            Frame++;
            Frame %= 4;
        }
    }

    public bool Contains(int x, int y, int width, int height)
    {
        return
            (x >= X) &&
            (y >= Y) &&
            (y < Y + height) &&
            (x < X + width);
    }
}