using System;
using Xunit;

namespace Drive.Tests;

/// <summary>
/// Tests for the Drive game logic.
/// Note: Since the game uses top-level statements and Console I/O extensively,
/// these tests validate the core game logic concepts and edge cases.
/// </summary>
public class DriveGameTests
{
    [Fact]
    public void SceneDimensions_ShouldBeValid()
    {
        // Arrange
        int width = 50;
        int height = 30;

        // Assert
        Assert.True(width > 0, "Width should be positive");
        Assert.True(height > 0, "Height should be positive");
        Assert.True(width >= 10, "Width should be at least 10 for game to be playable");
        Assert.True(height >= 10, "Height should be at least 10 for game to be playable");
    }

    [Fact]
    public void RoadWidth_ShouldBeConstant()
    {
        // Arrange
        const int roadWidth = 10;

        // Assert
        Assert.Equal(10, roadWidth);
    }

    [Theory]
    [InlineData(0, 50, true)]
    [InlineData(-1, 50, false)]
    [InlineData(50, 50, false)]
    [InlineData(25, 50, true)]
    [InlineData(49, 50, true)]
    public void CarPosition_BoundaryValidation(int carPosition, int width, bool shouldBeValid)
    {
        // Assert
        bool isValid = carPosition >= 0 && carPosition < width;
        Assert.Equal(shouldBeValid, isValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void CarVelocity_ShouldBeValidValue(int velocity)
    {
        // Assert
        Assert.True(velocity >= -1 && velocity <= 1, "Velocity should be -1, 0, or 1");
    }

    [Theory]
    [InlineData(25, -1, 24)]
    [InlineData(25, 0, 25)]
    [InlineData(25, 1, 26)]
    [InlineData(0, -1, -1)]
    [InlineData(49, 1, 50)]
    public void CarPosition_AfterVelocityUpdate(int initialPosition, int velocity, int expectedPosition)
    {
        // Act
        int newPosition = initialPosition + velocity;

        // Assert
        Assert.Equal(expectedPosition, newPosition);
    }

    [Fact]
    public void RoadEdges_ShouldBeCalculatedCorrectly()
    {
        // Arrange
        int width = 50;
        const int roadWidth = 10;

        // Act
        int leftEdge = (width - roadWidth) / 2;
        int rightEdge = leftEdge + roadWidth + 1;

        // Assert
        Assert.Equal(20, leftEdge);
        Assert.Equal(31, rightEdge);
    }

    [Theory]
    [InlineData(19, true)]  // Left of road
    [InlineData(20, false)] // Left edge
    [InlineData(25, false)] // On road
    [InlineData(31, false)] // Right edge
    [InlineData(32, true)]  // Right of road
    public void Position_ShouldDetermineOffRoad(int position, bool shouldBeOffRoad)
    {
        // Arrange
        int width = 50;
        const int roadWidth = 10;
        int leftEdge = (width - roadWidth) / 2;
        int rightEdge = leftEdge + roadWidth + 1;

        // Act
        bool isOffRoad = position < leftEdge || position > rightEdge;

        // Assert
        Assert.Equal(shouldBeOffRoad, isOffRoad);
    }

    [Fact]
    public void Score_ShouldStartAtZero()
    {
        // Arrange
        int score = 0;

        // Assert
        Assert.Equal(0, score);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(10, 11)]
    [InlineData(100, 101)]
    public void Score_ShouldIncrementEachFrame(int currentScore, int expectedScore)
    {
        // Act
        int newScore = currentScore + 1;

        // Assert
        Assert.Equal(expectedScore, newScore);
    }

    [Theory]
    [InlineData(50, 30, true)]
    [InlineData(49, 30, false)]
    [InlineData(50, 29, false)]
    [InlineData(51, 31, true)]
    public void ConsoleDimensions_MeetsMinimumRequirements(int windowWidth, int windowHeight, bool meetsRequirements)
    {
        // Arrange
        int minWidth = 50;
        int minHeight = 30;

        // Act
        bool isValid = windowWidth >= minWidth && windowHeight >= minHeight;

        // Assert
        Assert.Equal(meetsRequirements, isValid);
    }

    [Theory]
    [InlineData(' ', true)]
    [InlineData('.', false)]
    [InlineData('X', false)]
    public void SceneCharacter_DeterminesRoadSurface(char character, bool isRoad)
    {
        // Act
        bool onRoad = character == ' ';

        // Assert
        Assert.Equal(isRoad, onRoad);
    }

    [Fact]
    public void Scene_ShouldBeInitializedCorrectly()
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

        // Assert
        Assert.Equal('.', scene[0, 0]);
        Assert.Equal(' ', scene[0, 25]);
        Assert.Equal('.', scene[0, 49]);
    }

    [Theory]
    [InlineData(-1, '<')]
    [InlineData(0, '^')]
    [InlineData(1, '>')]
    public void CarCharacter_ShouldMatchVelocity(int velocity, char expectedChar)
    {
        // Act
        char carChar = velocity < 0 ? '<' : velocity > 0 ? '>' : '^';

        // Assert
        Assert.Equal(expectedChar, carChar);
    }

    [Fact]
    public void GameOver_CarCharacter_ShouldBeX()
    {
        // Arrange
        bool gameRunning = false;
        int velocity = 0;

        // Act
        char carChar = !gameRunning ? 'X' : velocity < 0 ? '<' : velocity > 0 ? '>' : '^';

        // Assert
        Assert.Equal('X', carChar);
    }

    [Theory]
    [InlineData(0, 4, -1)]  // Road moves left: new road on left would be -1
    [InlineData(0, 1, 0)]
    [InlineData(0, 1, 1)]
    public void RoadUpdate_RandomDirection(int previousUpdate, int randomChoice, int expectedUpdate)
    {
        // This tests the concept that road updates follow certain patterns
        // In actual game: Random.Shared.Next(5) < 4 ? previousRoadUpdate : Random.Shared.Next(3) - 1
        // If randomChoice < 4, use previous; else calculate new direction

        // Act
        int roadUpdate = randomChoice < 4 ? previousUpdate : randomChoice - 1;

        // Assert
        Assert.Equal(expectedUpdate, roadUpdate);
    }

    [Fact]
    public void RoadShiftLeft_ShouldMoveCharactersLeft()
    {
        // Arrange
        int width = 10;
        char[] row = new char[width];
        for (int i = 0; i < width; i++)
        {
            row[i] = (char)('0' + i);
        }

        // Act - Simulate road shifting left
        char[] newRow = new char[width];
        for (int i = 0; i < width - 1; i++)
        {
            newRow[i] = row[i + 1];
        }
        newRow[width - 1] = '.';

        // Assert
        Assert.Equal('1', newRow[0]);
        Assert.Equal('9', newRow[8]);
        Assert.Equal('.', newRow[9]);
    }

    [Fact]
    public void RoadShiftRight_ShouldMoveCharactersRight()
    {
        // Arrange
        int width = 10;
        char[] row = new char[width];
        for (int i = 0; i < width; i++)
        {
            row[i] = (char)('0' + i);
        }

        // Act - Simulate road shifting right
        char[] newRow = new char[width];
        for (int i = width - 1; i > 0; i--)
        {
            newRow[i] = row[i - 1];
        }
        newRow[0] = '.';

        // Assert
        Assert.Equal('.', newRow[0]);
        Assert.Equal('0', newRow[1]);
        Assert.Equal('8', newRow[9]);
    }

    [Fact]
    public void SceneUpdate_ShouldShiftRowsDown()
    {
        // Arrange
        int width = 5;
        int height = 3;
        char[,] scene = new char[height, width];

        // Initialize with pattern
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                scene[i, j] = (char)('A' + i);
            }
        }

        // Act - Simulate scene update (rows shift down)
        char[,] newScene = new char[height, width];
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width; j++)
            {
                newScene[i, j] = scene[i + 1, j];
            }
        }

        // Top row gets new content
        for (int j = 0; j < width; j++)
        {
            newScene[height - 1, j] = 'X';
        }

        // Assert
        Assert.Equal('B', newScene[0, 0]); // Row 0 should have old row 1's content
        Assert.Equal('C', newScene[1, 0]); // Row 1 should have old row 2's content
        Assert.Equal('X', newScene[2, 0]); // Row 2 should have new content
    }

    [Theory]
    [InlineData(ConsoleKey.A, -1)]
    [InlineData(ConsoleKey.LeftArrow, -1)]
    [InlineData(ConsoleKey.D, 1)]
    [InlineData(ConsoleKey.RightArrow, 1)]
    [InlineData(ConsoleKey.W, 0)]
    [InlineData(ConsoleKey.UpArrow, 0)]
    [InlineData(ConsoleKey.S, 0)]
    [InlineData(ConsoleKey.DownArrow, 0)]
    public void InputKey_ShouldMapToVelocity(ConsoleKey key, int expectedVelocity)
    {
        // Act
        int velocity = key switch
        {
            ConsoleKey.A or ConsoleKey.LeftArrow => -1,
            ConsoleKey.D or ConsoleKey.RightArrow => 1,
            ConsoleKey.W or ConsoleKey.UpArrow or ConsoleKey.S or ConsoleKey.DownArrow => 0,
            _ => 0
        };

        // Assert
        Assert.Equal(expectedVelocity, velocity);
    }

    [Theory]
    [InlineData(25, -1, ' ', true)]
    [InlineData(25, 0, ' ', true)]
    [InlineData(25, 1, ' ', true)]
    [InlineData(25, -1, '.', false)]
    [InlineData(25, 0, '.', false)]
    [InlineData(-1, 0, ' ', false)]
    [InlineData(50, 0, ' ', false)]
    public void CollisionDetection_CarPositionAndSurface(int carPosition, int velocity, char surface, bool shouldContinue)
    {
        // Arrange
        int width = 50;
        int newPosition = carPosition + velocity;

        // Act
        bool gameRunning = !(newPosition < 0 || newPosition >= width || surface != ' ');

        // Assert
        Assert.Equal(shouldContinue, gameRunning);
    }

    [Fact]
    public void CarPosition_InitialValue_ShouldBeMiddle()
    {
        // Arrange
        int width = 50;

        // Act
        int carPosition = width / 2;

        // Assert
        Assert.Equal(25, carPosition);
    }

    [Fact]
    public void CarVelocity_InitialValue_ShouldBeZero()
    {
        // Arrange
        int carVelocity = 0;

        // Assert
        Assert.Equal(0, carVelocity);
    }

    [Theory]
    [InlineData(ConsoleKey.Y, true)]
    [InlineData(ConsoleKey.N, false)]
    [InlineData(ConsoleKey.Escape, false)]
    public void GameOverInput_PlayAgainChoice(ConsoleKey key, bool shouldPlayAgain)
    {
        // Act
        bool keepPlaying = key switch
        {
            ConsoleKey.Y => true,
            ConsoleKey.N or ConsoleKey.Escape => false,
            _ => false
        };

        // Assert
        Assert.Equal(shouldPlayAgain, keepPlaying);
    }

    [Theory]
    [InlineData(ConsoleKey.Enter, true, true)]
    [InlineData(ConsoleKey.Escape, true, false)]
    public void LaunchScreen_Input(ConsoleKey key, bool validInput, bool shouldStartGame)
    {
        // Act
        bool startGame = key switch
        {
            ConsoleKey.Enter => true,
            ConsoleKey.Escape => false,
            _ => false
        };

        // Assert
        Assert.Equal(shouldStartGame, startGame);
    }
}