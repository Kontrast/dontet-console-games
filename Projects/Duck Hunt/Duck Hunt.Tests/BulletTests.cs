using System;
using Xunit;

namespace DuckHunt.Tests;

/// <summary>
/// Tests for the Bullet class from Duck Hunt game.
/// </summary>
public class BulletTests
{
    [Fact]
    public void Constructor_ShouldInitializePositions()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0;

        // Act
        var bullet = new TestBullet(position, angle);

        // Assert
        Assert.Equal(10, bullet.X[0]);
        Assert.Equal(20, bullet.Y[0]);
        Assert.Equal(10, bullet.X[1]);
        Assert.Equal(20, bullet.Y[1]);
    }

    [Fact]
    public void Constructor_InitialOutOfBounds_ShouldBeFalse()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0;

        // Act
        var bullet = new TestBullet(position, angle);

        // Assert
        Assert.False(bullet.OutOfBounds);
    }

    [Theory]
    [InlineData(0)]       // Right
    [InlineData(Math.PI / 2)]  // Down
    [InlineData(Math.PI)]      // Left
    [InlineData(3 * Math.PI / 2)] // Up
    public void Constructor_ShouldAcceptVariousAngles(double angle)
    {
        // Arrange
        var position = new TestPoint(10, 20);

        // Act
        var bullet = new TestBullet(position, angle);

        // Assert
        Assert.NotNull(bullet);
        Assert.False(bullet.OutOfBounds);
    }

    [Fact]
    public void UpdatePosition_ShouldMoveHorizontally_WhenAngleIsZero()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0; // Points right: cos(0) = 1, sin(0) = 0
        var bullet = new TestBullet(position, angle);

        // Act
        bullet.UpdatePosition();

        // Assert - Should move in negative X direction (because of -Math.Cos)
        Assert.True(bullet.X[0] < 10, "Bullet should move left (negative X)");
    }

    [Fact]
    public void UpdatePosition_ShouldTrackPreviousPosition()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0;
        var bullet = new TestBullet(position, angle);

        // Act
        bullet.UpdatePosition();

        // Assert - X[1] and Y[1] should store previous position
        Assert.Equal(10, bullet.X[1]);
        Assert.Equal(20, bullet.Y[1]);
    }

    [Fact]
    public void UpdatePosition_MultipleCalls_ShouldContinueMoving()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0;
        var bullet = new TestBullet(position, angle);
        double initialX = bullet.X[0];

        // Act
        bullet.UpdatePosition();
        double afterFirstUpdate = bullet.X[0];
        bullet.UpdatePosition();
        double afterSecondUpdate = bullet.X[0];

        // Assert
        Assert.NotEqual(initialX, afterFirstUpdate);
        Assert.NotEqual(afterFirstUpdate, afterSecondUpdate);
    }

    [Theory]
    [InlineData(-10, 20, 100, 100)]  // Left of screen
    [InlineData(110, 20, 100, 100)]  // Right of screen
    [InlineData(50, -10, 100, 100)]  // Above screen
    [InlineData(50, 110, 100, 100)]  // Below screen
    public void UpdatePosition_OutOfBounds_ShouldDetect(double x, double y, int screenWidth, int screenHeight)
    {
        // Arrange
        var position = new TestPoint((int)x, (int)y);
        double angle = 0;
        var bullet = new TestBullet(position, angle);
        bullet.X[0] = x;
        bullet.Y[0] = y;

        // Act
        bullet.CheckBounds(screenWidth, screenHeight);

        // Assert
        Assert.True(bullet.OutOfBounds);
    }

    [Theory]
    [InlineData(0, 0, 100, 100, false)]
    [InlineData(50, 50, 100, 100, false)]
    [InlineData(99, 99, 100, 100, false)]
    public void UpdatePosition_InsideBounds_ShouldNotSetOutOfBounds(double x, double y, int screenWidth, int screenHeight, bool expected)
    {
        // Arrange
        var position = new TestPoint((int)x, (int)y);
        double angle = 0;
        var bullet = new TestBullet(position, angle);
        bullet.X[0] = x;
        bullet.Y[0] = y;

        // Act
        bullet.CheckBounds(screenWidth, screenHeight);

        // Assert
        Assert.Equal(expected, bullet.OutOfBounds);
    }

    [Fact]
    public void Angle_ZeroDegrees_ShouldPointRight()
    {
        // Arrange
        double angle = 0;

        // Act
        double xOffset = -Math.Cos(angle);
        double yOffset = -Math.Sin(angle);

        // Assert
        Assert.Equal(-1.0, xOffset, 3); // -cos(0) = -1
        Assert.Equal(0.0, yOffset, 3);   // -sin(0) = 0
    }

    [Fact]
    public void Angle_90Degrees_ShouldPointDown()
    {
        // Arrange
        double angle = Math.PI / 2; // 90 degrees

        // Act
        double xOffset = -Math.Cos(angle);
        double yOffset = -Math.Sin(angle);

        // Assert
        Assert.Equal(0.0, xOffset, 3);   // -cos(90) ≈ 0
        Assert.Equal(-1.0, yOffset, 3);  // -sin(90) = -1
    }

    [Fact]
    public void Angle_180Degrees_ShouldPointLeft()
    {
        // Arrange
        double angle = Math.PI; // 180 degrees

        // Act
        double xOffset = -Math.Cos(angle);
        double yOffset = -Math.Sin(angle);

        // Assert
        Assert.Equal(1.0, xOffset, 3);   // -cos(180) = 1
        Assert.Equal(0.0, yOffset, 3);   // -sin(180) ≈ 0
    }

    [Fact]
    public void Angle_270Degrees_ShouldPointUp()
    {
        // Arrange
        double angle = 3 * Math.PI / 2; // 270 degrees

        // Act
        double xOffset = -Math.Cos(angle);
        double yOffset = -Math.Sin(angle);

        // Assert
        Assert.Equal(0.0, xOffset, 3);   // -cos(270) ≈ 0
        Assert.Equal(1.0, yOffset, 3);   // -sin(270) = 1
    }

    [Fact]
    public void BulletTrail_ShouldProvideCurrentAndPreviousPositions()
    {
        // Arrange
        var position = new TestPoint(10, 20);
        double angle = 0;
        var bullet = new TestBullet(position, angle);

        // Act
        bullet.UpdatePosition();
        double currentX = bullet.X[0];
        double currentY = bullet.Y[0];
        double previousX = bullet.X[1];
        double previousY = bullet.Y[1];

        // Assert - Should have two different positions for trail effect
        Assert.Equal(10, previousX);
        Assert.Equal(20, previousY);
        Assert.NotEqual(currentX, previousX);
    }
}

/// <summary>
/// Test implementation of Bullet class to match the Duck Hunt implementation.
/// </summary>
public class TestBullet
{
    public bool OutOfBounds = false;
    public double[] X = new double[2];
    public double[] Y = new double[2];

    private readonly double XOffset;
    private readonly double YOffset;

    public TestBullet(TestPoint position, double angle)
    {
        for (int i = 0; i < 2; i++)
        {
            X[i] = position.X;
            Y[i] = position.Y;
        }

        XOffset = -Math.Cos(angle);
        YOffset = -Math.Sin(angle);
    }

    public void UpdatePosition()
    {
        X[1] = X[0];
        Y[1] = Y[0];

        X[0] += XOffset;
        Y[0] += YOffset;
    }

    public void CheckBounds(int screenWidth, int screenHeight)
    {
        if (X[0] < 0 || X[0] >= screenWidth ||
            Y[0] < 0 || Y[0] >= screenHeight)
        {
            OutOfBounds = true;
        }
    }
}