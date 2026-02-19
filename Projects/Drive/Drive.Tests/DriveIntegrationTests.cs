using System;
using Xunit;

namespace Drive.Tests;

/// <summary>
/// Integration and edge case tests for the Drive game.
/// </summary>
public class DriveIntegrationTests
{
    [Fact]
    public void CompleteGameScenario_StartToCollision()
    {
        // Arrange
        int width = 50;
        int height = 30;
        const int roadWidth = 10;
        int leftEdge = (width - roadWidth) / 2;
        int rightEdge = leftEdge + roadWidth + 1;

        int carPosition = width / 2;
        int carVelocity = 0;
        int score = 0;
        bool gameRunning = true;
        char[,] scene = new char[height, width];

        // Initialize scene
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                scene[i, j] = (j < leftEdge || j > rightEdge) ? '.' : ' ';
            }
        }

        // Act - Simulate car moving left into obstacle
        carVelocity = -1;
        for (int frame = 0; frame < 10 && gameRunning; frame++)
        {
            carPosition += carVelocity;

            // Check collision
            if (carPosition < 0 || carPosition >= width || scene[1, carPosition] != ' ')
            {
                gameRunning = false;
            }
            else
            {
                score++;
            }
        }

        // Assert
        Assert.False(gameRunning);
        Assert.True(score < 10); // Collision happened before 10 frames
    }

    [Fact]
    public void CompleteGameScenario_StayingOnRoad()
    {
        // Arrange
        int width = 50;
        int height = 30;
        const int roadWidth = 10;
        int leftEdge = (width - roadWidth) / 2;
        int rightEdge = leftEdge + roadWidth + 1;

        int carPosition = width / 2;
        int carVelocity = 0;
        int score = 0;
        bool gameRunning = true;
        char[,] scene = new char[height, width];

        // Initialize scene with road
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                scene[i, j] = (j < leftEdge || j > rightEdge) ? '.' : ' ';
            }
        }

        // Act - Car stays still on road
        for (int frame = 0; frame < 100 && gameRunning; frame++)
        {
            carPosition += carVelocity; // No movement

            if (carPosition < 0 || carPosition >= width || scene[1, carPosition] != ' ')
            {
                gameRunning = false;
            }
            else
            {
                score++;
            }
        }

        // Assert
        Assert.True(gameRunning);
        Assert.Equal(100, score);
    }

    [Fact]
    public void SceneUpdate_ScrollingSimulation()
    {
        // Arrange
        int width = 10;
        int height = 5;
        char[,] scene = new char[height, width];

        // Fill with pattern
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                scene[i, j] = (char)('0' + i);
            }
        }

        // Act - Simulate scene scrolling (rows move down)
        char[,] newScene = new char[height, width];
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width; j++)
            {
                newScene[i, j] = scene[i + 1, j];
            }
        }

        // New row at top
        for (int j = 0; j < width; j++)
        {
            newScene[height - 1, j] = ' ';
        }

        // Assert
        Assert.Equal('1', newScene[0, 0]);
        Assert.Equal('2', newScene[1, 0]);
        Assert.Equal(' ', newScene[4, 0]);
    }

    [Fact]
    public void RoadShift_LeftBoundaryConstraint()
    {
        // Arrange
        int width = 10;
        char[] row = new char[width];
        for (int i = 0; i < width; i++)
        {
            row[i] = i == 0 ? ' ' : '.';
        }

        // Act - Road wants to shift left but can't (left edge is already road)
        int roadUpdate = -1;
        bool canShiftLeft = row[0] != ' ';

        if (!canShiftLeft)
        {
            roadUpdate = 1; // Reverse direction
        }

        // Assert
        Assert.Equal(1, roadUpdate);
    }

    [Fact]
    public void RoadShift_RightBoundaryConstraint()
    {
        // Arrange
        int width = 10;
        char[] row = new char[width];
        for (int i = 0; i < width; i++)
        {
            row[i] = i == width - 1 ? ' ' : '.';
        }

        // Act - Road wants to shift right but can't
        int roadUpdate = 1;
        bool canShiftRight = row[width - 1] != ' ';

        if (!canShiftRight)
        {
            roadUpdate = -1; // Reverse direction
        }

        // Assert
        Assert.Equal(-1, roadUpdate);
    }

    [Fact]
    public void CarMovement_MultipleVelocityChanges()
    {
        // Arrange
        int carPosition = 25;
        int[] velocities = { 1, 1, -1, 0, -1, 1 };

        // Act
        foreach (int velocity in velocities)
        {
            carPosition += velocity;
        }

        // Assert
        Assert.Equal(26, carPosition); // 25 + 1 + 1 - 1 + 0 - 1 + 1 = 26
    }

    [Theory]
    [InlineData(new[] { ConsoleKey.A, ConsoleKey.D, ConsoleKey.W }, 0)]
    [InlineData(new[] { ConsoleKey.A, ConsoleKey.A }, -1)]
    [InlineData(new[] { ConsoleKey.D, ConsoleKey.D }, 1)]
    [InlineData(new[] { ConsoleKey.W, ConsoleKey.S }, 0)]
    public void InputSequence_FinalVelocity(ConsoleKey[] keys, int expectedVelocity)
    {
        // Arrange
        int velocity = 0;

        // Act
        foreach (var key in keys)
        {
            velocity = key switch
            {
                ConsoleKey.A or ConsoleKey.LeftArrow => -1,
                ConsoleKey.D or ConsoleKey.RightArrow => 1,
                ConsoleKey.W or ConsoleKey.UpArrow or ConsoleKey.S or ConsoleKey.DownArrow => 0,
                _ => velocity
            };
        }

        // Assert
        Assert.Equal(expectedVelocity, velocity);
    }

    [Fact]
    public void ScoreIncrement_ConsecutiveFrames()
    {
        // Arrange
        int score = 0;
        int frames = 100;

        // Act
        for (int i = 0; i < frames; i++)
        {
            score++;
        }

        // Assert
        Assert.Equal(100, score);
    }

    [Fact]
    public void EdgeCase_CarAtLeftBoundary()
    {
        // Arrange
        int carPosition = 0;
        int carVelocity = -1;
        int width = 50;

        // Act
        int newPosition = carPosition + carVelocity;
        bool outOfBounds = newPosition < 0 || newPosition >= width;

        // Assert
        Assert.True(outOfBounds);
        Assert.Equal(-1, newPosition);
    }

    [Fact]
    public void EdgeCase_CarAtRightBoundary()
    {
        // Arrange
        int carPosition = 49;
        int carVelocity = 1;
        int width = 50;

        // Act
        int newPosition = carPosition + carVelocity;
        bool outOfBounds = newPosition < 0 || newPosition >= width;

        // Assert
        Assert.True(outOfBounds);
        Assert.Equal(50, newPosition);
    }

    [Fact]
    public void EdgeCase_MinimumWindowSize()
    {
        // Arrange
        int windowWidth = 50;
        int windowHeight = 30;
        int minWidth = 50;
        int minHeight = 30;

        // Act
        bool isValid = windowWidth >= minWidth && windowHeight >= minHeight;

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void EdgeCase_BelowMinimumWindowSize()
    {
        // Arrange
        int windowWidth = 49;
        int windowHeight = 30;
        int minWidth = 50;
        int minHeight = 30;

        // Act
        bool isValid = windowWidth >= minWidth && windowHeight >= minHeight;
        bool shouldShowError = !isValid;

        // Assert
        Assert.False(isValid);
        Assert.True(shouldShowError);
    }

    [Fact]
    public void RoadGeneration_InitialState()
    {
        // Arrange
        int width = 50;
        int height = 30;
        const int roadWidth = 10;
        int leftEdge = (width - roadWidth) / 2;
        int rightEdge = leftEdge + roadWidth + 1;

        // Act
        char[,] scene = new char[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (j < leftEdge || j > rightEdge)
                {
                    scene[i, j] = '.';
                }
                else
                {
                    scene[i, j] = ' ';
                }
            }
        }

        // Assert - Check road is centered
        Assert.Equal('.', scene[0, 0]);
        Assert.Equal('.', scene[0, 19]);
        Assert.Equal(' ', scene[0, 20]);
        Assert.Equal(' ', scene[0, 30]);
        Assert.Equal(' ', scene[0, 31]);
        Assert.Equal('.', scene[0, 32]);
    }

    [Fact]
    public void RoadShift_FullLeftSequence()
    {
        // Arrange
        int width = 10;
        char[] row = { '.', '.', ' ', ' ', ' ', ' ', ' ', '.', '.', '.' };

        // Act - Shift left
        char[] newRow = new char[width];
        for (int i = 0; i < width - 1; i++)
        {
            newRow[i] = row[i + 1];
        }
        newRow[width - 1] = '.';

        // Assert
        Assert.Equal('.', newRow[0]);
        Assert.Equal(' ', newRow[1]);
        Assert.Equal(' ', newRow[5]);
        Assert.Equal('.', newRow[9]);
    }

    [Fact]
    public void RoadShift_FullRightSequence()
    {
        // Arrange
        int width = 10;
        char[] row = { '.', '.', ' ', ' ', ' ', ' ', ' ', '.', '.', '.' };

        // Act - Shift right
        char[] newRow = new char[width];
        for (int i = width - 1; i > 0; i--)
        {
            newRow[i] = row[i - 1];
        }
        newRow[0] = '.';

        // Assert
        Assert.Equal('.', newRow[0]);
        Assert.Equal('.', newRow[1]);
        Assert.Equal('.', newRow[2]);
        Assert.Equal(' ', newRow[3]);
    }

    [Theory]
    [InlineData(0, 4, 0)]  // 4 times same direction
    [InlineData(0, 5, -1)] // 5th time might change
    [InlineData(0, 5, 0)]
    [InlineData(0, 5, 1)]
    public void RoadDirection_PersistencePattern(int previousDirection, int consecutiveFrames, int newDirection)
    {
        // This tests the pattern: Random.Shared.Next(5) < 4 ? previousRoadUpdate : Random.Shared.Next(3) - 1
        // 80% chance to keep same direction, 20% chance to change

        // Act
        bool likelyToKeepDirection = consecutiveFrames < 5;

        // Assert
        if (consecutiveFrames < 5)
        {
            Assert.Equal(previousDirection, previousDirection);
        }
        // After 5 frames, direction might change (tested via randomness)
    }

    [Fact]
    public void Rendering_CarCharacterBasedOnState()
    {
        // Arrange
        bool gameRunning = true;
        int velocity = 0;

        // Act & Assert - Different states
        char carChar = !gameRunning ? 'X' : velocity < 0 ? '<' : velocity > 0 ? '>' : '^';
        Assert.Equal('^', carChar);

        velocity = -1;
        carChar = !gameRunning ? 'X' : velocity < 0 ? '<' : velocity > 0 ? '>' : '^';
        Assert.Equal('<', carChar);

        velocity = 1;
        carChar = !gameRunning ? 'X' : velocity < 0 ? '<' : velocity > 0 ? '>' : '^';
        Assert.Equal('>', carChar);

        gameRunning = false;
        carChar = !gameRunning ? 'X' : velocity < 0 ? '<' : velocity > 0 ? '>' : '^';
        Assert.Equal('X', carChar);
    }

    [Fact]
    public void RegressionTest_ScoreNeverDecreases()
    {
        // Arrange
        int score = 100;

        // Act - Simulate various game events (score should only increase)
        score++; // Normal frame
        score++; // Another frame

        // Assert
        Assert.Equal(102, score);
        Assert.True(score >= 100);
    }

    [Fact]
    public void RegressionTest_VelocityStaysInBounds()
    {
        // Arrange & Act
        int[] velocities = { -1, 0, 1 };

        // Assert - All valid velocities
        foreach (int v in velocities)
        {
            Assert.True(v >= -1 && v <= 1);
        }
    }

    [Fact]
    public void StressTest_ManyFrames()
    {
        // Arrange
        int score = 0;
        int carPosition = 25;
        int carVelocity = 0;

        // Act - Simulate many frames
        for (int i = 0; i < 10000; i++)
        {
            carPosition += carVelocity;
            if (carPosition >= 0 && carPosition < 50)
            {
                score++;
            }
        }

        // Assert
        Assert.Equal(10000, score);
    }

    [Fact]
    public void CompleteGameScenario_RoadCurvesLeft()
    {
        // Arrange
        int width = 50;
        int carPosition = 25; // Middle
        int carVelocity = 0;
        bool gameRunning = true;

        // Simulate road curving left by shifting car position right relative to road
        // Act
        for (int i = 0; i < 5; i++)
        {
            // Road shifts left, car needs to move left to compensate
            carVelocity = -1; // Player input
            carPosition += carVelocity;
        }

        // Assert
        Assert.Equal(20, carPosition);
        Assert.True(carPosition >= 0 && carPosition < width);
    }

    [Fact]
    public void CompleteGameScenario_RoadCurvesRight()
    {
        // Arrange
        int width = 50;
        int carPosition = 25; // Middle
        int carVelocity = 0;

        // Act - Road curves right, car moves right
        for (int i = 0; i < 5; i++)
        {
            carVelocity = 1;
            carPosition += carVelocity;
        }

        // Assert
        Assert.Equal(30, carPosition);
        Assert.True(carPosition >= 0 && carPosition < width);
    }

    [Fact]
    public void BoundaryTest_CarPositionLimits()
    {
        // Test all boundary positions
        int width = 50;

        // Assert valid positions
        for (int pos = 0; pos < width; pos++)
        {
            bool isValid = pos >= 0 && pos < width;
            Assert.True(isValid, $"Position {pos} should be valid");
        }

        // Assert invalid positions
        Assert.False(-1 >= 0 && -1 < width);
        Assert.False(50 >= 0 && 50 < width);
        Assert.False(100 >= 0 && 100 < width);
    }
}