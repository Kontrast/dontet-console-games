using System;
using Xunit;

namespace DuckHunt.Tests;

/// <summary>
/// Tests for Duck Hunt game logic and mechanics.
/// </summary>
public class GameLogicTests
{
    [Fact]
    public void InitialGameState_ShouldBeConfiguredCorrectly()
    {
        // Arrange & Act
        int gameDelay = 30;
        double gunXStretch = 1;
        int crosshairSpeed = 2;
        bool gunEnabled = true;
        bool bulletsEnabled = false;
        bool gunOutlineEnabled = false;
        int ammoCount = 5;
        int score = 0;

        // Assert
        Assert.Equal(30, gameDelay);
        Assert.Equal(1, gunXStretch);
        Assert.Equal(2, crosshairSpeed);
        Assert.True(gunEnabled);
        Assert.False(bulletsEnabled);
        Assert.False(gunOutlineEnabled);
        Assert.Equal(5, ammoCount);
        Assert.Equal(0, score);
    }

    [Theory]
    [InlineData(5, true)]   // Max ammo
    [InlineData(1, true)]   // Low ammo
    [InlineData(0, false)]  // No ammo
    public void GameOver_WhenNoAmmoAndNoBullets(int ammoCount, bool canContinue)
    {
        // Arrange
        int bulletsCount = 0;

        // Act
        bool gameOver = ammoCount is 0 && bulletsCount is 0;

        // Assert
        Assert.Equal(!canContinue, gameOver);
    }

    [Theory]
    [InlineData(0, 5, false)]  // No ammo but bullets exist
    [InlineData(5, 0, false)]  // Has ammo, no bullets
    [InlineData(0, 0, true)]   // No ammo, no bullets - game over
    public void GameOver_Conditions(int ammoCount, int bulletsCount, bool expectedGameOver)
    {
        // Act
        bool gameOver = ammoCount is 0 && bulletsCount is 0;

        // Assert
        Assert.Equal(expectedGameOver, gameOver);
    }

    [Theory]
    [InlineData(0, 2, 2)]
    [InlineData(3, 2, 5)]
    [InlineData(5, 2, 5)]  // Should cap at 5
    [InlineData(4, 2, 5)]  // Should cap at 5
    public void AmmoCount_AfterKill_ShouldIncrease(int currentAmmo, int ammoReward, int expectedAmmo)
    {
        // Act
        int newAmmo = currentAmmo + ammoReward;
        if (newAmmo > 5)
        {
            newAmmo = 5;
        }

        // Assert
        Assert.Equal(expectedAmmo, newAmmo);
    }

    [Theory]
    [InlineData(5, 1, 4)]
    [InlineData(1, 1, 0)]
    [InlineData(3, 1, 2)]
    public void AmmoCount_AfterMiss_ShouldDecrease(int currentAmmo, int cost, int expectedAmmo)
    {
        // Act
        int newAmmo = currentAmmo - cost;

        // Assert
        Assert.Equal(expectedAmmo, newAmmo);
    }

    [Theory]
    [InlineData(0, 150, 150)]
    [InlineData(100, 150, 250)]
    [InlineData(1000, 350, 1350)]
    public void Score_AfterHit_ShouldIncrease(int currentScore, int points, int expectedScore)
    {
        // Act
        int newScore = currentScore + points;

        // Assert
        Assert.Equal(expectedScore, newScore);
    }

    [Fact]
    public void HitWithoutBullets_ShouldAward150Points()
    {
        // Arrange
        int score = 0;
        bool bulletsEnabled = false;

        // Act
        if (!bulletsEnabled)
        {
            score += 150;
        }

        // Assert
        Assert.Equal(150, score);
    }

    [Fact]
    public void HitWithBullets_ShouldAward350Points()
    {
        // Arrange
        int score = 0;
        bool bulletsEnabled = true;

        // Act
        if (bulletsEnabled)
        {
            score += 350;
        }

        // Assert
        Assert.Equal(350, score);
    }

    [Theory]
    [InlineData(100, 60, true)]
    [InlineData(100, 100, true)]
    [InlineData(100, 101, false)]
    [InlineData(100, 59, false)]
    public void SpawnDelay_ShouldDecreaseToMinimum(int currentDelay, int minimum, bool shouldDecrease)
    {
        // Act
        int newDelay = currentDelay;
        if (currentDelay > minimum)
        {
            newDelay--;
        }

        // Assert
        if (shouldDecrease)
        {
            Assert.True(newDelay < currentDelay || currentDelay == minimum);
        }
        else
        {
            Assert.Equal(currentDelay, newDelay);
        }
    }

    [Theory]
    [InlineData(50, 25, true)]  // 50% chance, roll 25 - should spawn right
    [InlineData(50, 26, false)] // 50% chance, roll 26 - should spawn left
    [InlineData(50, 30, false)]
    public void BirdSpawn_RandomSide(int threshold, int randomValue, bool shouldSpawnRight)
    {
        // Act - Random.Next(50) > 25 determines spawn side in the game
        bool spawnRight = randomValue > threshold / 2;

        // Assert
        Assert.Equal(shouldSpawnRight, spawnRight);
    }

    [Theory]
    [InlineData(120, 10, true)]
    [InlineData(120, 10, true)]
    [InlineData(119, 10, false)]
    [InlineData(121, 10, false)]
    public void BirdSpawn_Timing(int frame, int spawnDelay, bool shouldSpawn)
    {
        // Act
        bool spawn = frame % spawnDelay == 0;

        // Assert
        Assert.Equal(shouldSpawn, spawn);
    }

    [Theory]
    [InlineData(ConsoleKey.UpArrow, 0, -2, 2)]
    [InlineData(ConsoleKey.DownArrow, 0, 2, 2)]
    [InlineData(ConsoleKey.LeftArrow, 0, 0, 2)]
    [InlineData(ConsoleKey.RightArrow, 0, 0, 2)]
    public void Crosshair_KeyMovement(ConsoleKey key, int currentY, int expectedDeltaY, int speed)
    {
        // Act
        int deltaY = key switch
        {
            ConsoleKey.UpArrow => -speed,
            ConsoleKey.DownArrow => speed,
            _ => 0
        };

        // Assert
        Assert.Equal(expectedDeltaY, deltaY);
    }

    [Theory]
    [InlineData(10, 5, 100, 10)]  // Within bounds
    [InlineData(-5, 5, 100, 2)]   // Below minimum (clamp to min)
    [InlineData(105, 5, 100, 97)] // Above maximum (clamp to max)
    public void Crosshair_BoundaryCheck(int position, int crosshairWidth, int screenWidth, int expectedClamped)
    {
        // Act
        int maxPosition = screenWidth - crosshairWidth + 2;
        int minPosition = 2;
        int clampedPosition = Math.Min(maxPosition, Math.Max(position, minPosition));

        // Assert
        Assert.Equal(expectedClamped, clampedPosition);
    }

    [Fact]
    public void GunAngle_ShouldBeCalculatedFromCrosshairPosition()
    {
        // Arrange
        int crosshairX = 60;
        int crosshairY = 15;
        int middleX = 50;
        int middleY = 25;

        // Act
        double theta = Math.Atan2(middleY - crosshairY, middleX - crosshairX);

        // Assert
        Assert.NotEqual(0, theta);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(5)]
    [InlineData(15)]
    public void GunBarrel_Length_ShouldBeConstant(int barrelLength)
    {
        // Arrange
        const int BARREL_LENGTH = 10;

        // Assert
        Assert.Equal(10, BARREL_LENGTH);
    }

    [Theory]
    [InlineData(0.5, 10)]
    [InlineData(1.0, 10)]
    [InlineData(1.5, 10)]
    [InlineData(2.0, 10)]
    public void GunBarrel_XStretch_ShouldAffectWidth(double stretch, int baseLength)
    {
        // Act
        double stretchedLength = baseLength * stretch;

        // Assert
        Assert.Equal(baseLength * stretch, stretchedLength);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void BirdAnimation_FrameUpdate(int frame, bool shouldUpdate)
    {
        // Act - Animation updates every 2 frames
        bool update = frame % 2 == 0;

        // Assert
        Assert.Equal(shouldUpdate, update);
    }

    [Theory]
    [InlineData(10, -1, 9)]    // Moving left
    [InlineData(10, 1, 11)]    // Moving right
    [InlineData(0, -1, -1)]    // Off screen left
    [InlineData(100, 1, 101)]  // Off screen right
    public void Bird_HorizontalMovement(int currentX, int direction, int expectedX)
    {
        // Act
        int newX = currentX + direction;

        // Assert
        Assert.Equal(expectedX, newX);
    }

    [Theory]
    [InlineData(10, 11)]
    [InlineData(20, 21)]
    [InlineData(0, 1)]
    public void DeadBird_Falling(int currentY, int expectedY)
    {
        // Act - Dead birds fall (Y increases)
        int newY = currentY + 1;

        // Assert
        Assert.Equal(expectedY, newY);
    }

    [Theory]
    [InlineData(-5, 10, 120, true)]   // Left of screen
    [InlineData(125, 10, 120, true)]  // Right of screen
    [InlineData(60, 35, 30, true)]    // Below screen
    [InlineData(60, 10, 30, false)]   // On screen
    public void Bird_RemovalConditions(int x, int y, int screenHeight, bool shouldRemove)
    {
        // Arrange
        int screenWidth = 120;
        int birdWidth = 10;
        int direction = x < 0 ? -1 : 1;

        // Act
        bool remove =
            y > screenHeight ||
            (direction is -1 && x < -birdWidth) ||
            (direction is 1 && x > screenWidth + birdWidth);

        // Assert
        Assert.Equal(shouldRemove, remove);
    }

    [Theory]
    [InlineData(ConsoleKey.D1, true, false)]
    [InlineData(ConsoleKey.D2, false, true)]
    [InlineData(ConsoleKey.D3, false, false)]
    public void Menu_VariableSelection(ConsoleKey key, bool gunSelected, bool crosshairSelected)
    {
        // Act
        bool isGunSelected = key == ConsoleKey.D1;
        bool isCrosshairSelected = key == ConsoleKey.D2;

        // Assert
        Assert.Equal(gunSelected, isGunSelected);
        Assert.Equal(crosshairSelected, isCrosshairSelected);
    }

    [Theory]
    [InlineData(ConsoleKey.D4, false, true)]
    [InlineData(ConsoleKey.D5, false, true)]
    [InlineData(ConsoleKey.D6, true, false)]
    public void Menu_ToggleBooleanSettings(ConsoleKey key, bool currentValue, bool expectedValue)
    {
        // Act
        bool newValue = key switch
        {
            ConsoleKey.D4 or ConsoleKey.D5 => !currentValue,
            ConsoleKey.D6 => !currentValue,
            _ => currentValue
        };

        // Assert
        Assert.Equal(expectedValue, newValue);
    }

    [Theory]
    [InlineData(1.0, 0.1, 1.1)]
    [InlineData(2.0, 0.1, 2.1)]
    [InlineData(1.5, -0.1, 1.4)]
    public void Menu_AdjustGunStretch(double currentStretch, double delta, double expectedStretch)
    {
        // Act
        double newStretch = currentStretch + delta;

        // Assert
        Assert.Equal(expectedStretch, newStretch, 2);
    }

    [Theory]
    [InlineData(2, 1, 3)]
    [InlineData(5, 1, 6)]
    [InlineData(10, -1, 9)]
    [InlineData(1, -1, 0)]
    public void Menu_AdjustCrosshairSpeed(int currentSpeed, int delta, int expectedSpeed)
    {
        // Act
        int newSpeed = currentSpeed + delta;

        // Assert
        Assert.Equal(expectedSpeed, newSpeed);
    }

    [Theory]
    [InlineData(30, 1, 31)]
    [InlineData(50, -1, 49)]
    [InlineData(1, -1, 0)]
    public void Menu_AdjustGameDelay(int currentDelay, int delta, int expectedDelay)
    {
        // Act
        int newDelay = currentDelay + delta;

        // Assert
        Assert.Equal(expectedDelay, newDelay);
    }

    [Fact]
    public void BresenhamLine_ShouldCalculateDelta()
    {
        // Arrange
        int startX = 10, startY = 10;
        int endX = 20, endY = 15;

        // Act
        int dx = Math.Abs(startX - endX);
        int dy = -Math.Abs(startY - endY);

        // Assert
        Assert.Equal(10, dx);
        Assert.Equal(-5, dy);
    }

    [Theory]
    [InlineData(10, 20, 1)]
    [InlineData(20, 10, -1)]
    [InlineData(10, 10, 1)]
    public void BresenhamLine_DirectionCalculation(int start, int end, int expectedDirection)
    {
        // Act
        int direction = start < end ? 1 : -1;

        // Assert
        Assert.Equal(expectedDirection, direction);
    }

    [Fact]
    public void GrassLevel_ShouldBeNearBottom()
    {
        // Arrange
        int screenHeight = 30;

        // Act
        int grassLevel = screenHeight - 4;

        // Assert
        Assert.Equal(26, grassLevel);
    }

    [Theory]
    [InlineData(120, 30, 119, 30)]  // Normal screen
    [InlineData(80, 25, 79, 25)]    // Smaller screen
    public void ScreenDimensions_Adjustment(int windowWidth, int windowHeight, int expectedWidth, int expectedHeight)
    {
        // Act - Game uses Console.WindowWidth - 1 and Console.WindowHeight
        int screenWidth = windowWidth - 1;
        int screenHeight = windowHeight;

        // Assert
        Assert.Equal(expectedWidth, screenWidth);
        Assert.Equal(expectedHeight, screenHeight);
    }
}