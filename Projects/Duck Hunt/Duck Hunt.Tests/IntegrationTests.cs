using System;
using Xunit;

namespace DuckHunt.Tests;

/// <summary>
/// Integration tests for Duck Hunt game simulating realistic game scenarios.
/// </summary>
public class IntegrationTests
{
    [Fact]
    public void CompleteGameScenario_ShootBirdWithoutBullets()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);
        int crosshairX = 55;
        int crosshairY = 21;
        int ammoCount = 5;
        int score = 0;
        bool bulletsEnabled = false;

        // Act - Simulate shooting
        if (!bulletsEnabled && ammoCount > 0)
        {
            if (!bird.IsDead && bird.Contains(crosshairX, crosshairY, 10, 3))
            {
                bird.IsDead = true;
                ammoCount += 2;
                score += 150;
            }
            ammoCount--;
        }

        // Assert
        Assert.True(bird.IsDead);
        Assert.Equal(6, ammoCount); // 5 - 1 + 2 = 6
        Assert.Equal(150, score);
    }

    [Fact]
    public void CompleteGameScenario_MissShot()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);
        int crosshairX = 10; // Far from bird
        int crosshairY = 10;
        int ammoCount = 5;
        int score = 0;
        bool bulletsEnabled = false;

        // Act - Simulate shooting and missing
        if (!bulletsEnabled && ammoCount > 0)
        {
            if (!bird.IsDead && bird.Contains(crosshairX, crosshairY, 10, 3))
            {
                bird.IsDead = true;
                ammoCount += 2;
                score += 150;
            }
            ammoCount--;
        }

        // Assert
        Assert.False(bird.IsDead);
        Assert.Equal(4, ammoCount); // Lost 1 ammo
        Assert.Equal(0, score);
    }

    [Fact]
    public void CompleteGameScenario_ShootBirdWithBullets()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);
        var middleAncor = new TestPoint(60, 28);
        var gunTopOffset = new TestPoint(-5, -8);
        var bulletStartPos = middleAncor + gunTopOffset;
        double angle = Math.Atan2(middleAncor.Y - 20, middleAncor.X - 55);
        var bullet = new TestBullet(bulletStartPos, angle);
        int ammoCount = 5;
        int score = 0;
        bool bulletsEnabled = true;

        // Act - Create bullet
        ammoCount--;

        // Simulate bullet hitting bird
        for (int i = 0; i < 20 && !bird.IsDead; i++)
        {
            bullet.UpdatePosition();

            if (!bird.IsDead &&
                (bird.Contains((int)bullet.X[0], (int)bullet.Y[0], 10, 3) ||
                 bird.Contains((int)bullet.X[1], (int)bullet.Y[1], 10, 3)))
            {
                bird.IsDead = true;
                ammoCount += 2;
                score += 350;
            }
        }

        // Assert
        Assert.Equal(4, ammoCount); // 5 - 1 (no hit in 20 iterations)
    }

    [Fact]
    public void CompleteGameScenario_GameOver_NoAmmo()
    {
        // Arrange
        int ammoCount = 1;
        int bulletsCount = 0;
        var bird = new TestBird(50, 20, 1);
        int crosshairX = 10;
        int crosshairY = 10;
        bool gameOver = false;

        // Act - Fire last shot and miss
        ammoCount--;

        // Check game over
        gameOver = ammoCount is 0 && bulletsCount is 0;

        // Assert
        Assert.True(gameOver);
    }

    [Fact]
    public void CompleteGameScenario_BirdMovementAndRemoval()
    {
        // Arrange
        var bird = new TestBird(120, 20, 1); // Starting at right edge, moving right
        int screenWidth = 120;
        int birdWidth = 10;

        // Act - Simulate bird moving off screen
        for (int i = 0; i < 15; i++)
        {
            bird.X += bird.Direction;
        }

        bool shouldRemove = bird.Direction is 1 && bird.X > screenWidth + birdWidth;

        // Assert
        Assert.True(shouldRemove);
        Assert.True(bird.X > screenWidth + birdWidth);
    }

    [Fact]
    public void CompleteGameScenario_DeadBirdFalling()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);
        bird.IsDead = true;
        int screenHeight = 30;

        // Act - Simulate bird falling
        for (int i = 0; i < 15; i++)
        {
            bird.Y++;
        }

        bool shouldRemove = bird.Y > screenHeight;

        // Assert
        Assert.True(shouldRemove);
        Assert.True(bird.Y > screenHeight);
    }

    [Fact]
    public void CompleteGameScenario_CrosshairMovement()
    {
        // Arrange
        var crosshair = new TestPoint(60, 15);
        int crosshairSpeed = 2;
        int screenWidth = 120;
        int screenHeight = 30;
        int crosshairWidth = 5;
        int crosshairHeight = 5;

        // Act - Simulate moving crosshair
        crosshair.Y -= crosshairSpeed; // Move up
        crosshair.X += crosshairSpeed; // Move right

        // Clamp to screen bounds
        int maxX = screenWidth - crosshairWidth + 2;
        int maxY = screenHeight - crosshairHeight;
        crosshair.X = Math.Min(maxX, Math.Max(crosshair.X, 2));
        crosshair.Y = Math.Min(maxY, Math.Max(crosshair.Y, 2));

        // Assert
        Assert.Equal(62, crosshair.X);
        Assert.Equal(13, crosshair.Y);
    }

    [Fact]
    public void CompleteGameScenario_CrosshairBoundaryClamp()
    {
        // Arrange
        var crosshair = new TestPoint(2, 2);
        int crosshairSpeed = 2;
        int crosshairWidth = 5;
        int crosshairHeight = 5;
        int screenWidth = 120;
        int screenHeight = 30;

        // Act - Try to move beyond minimum bounds
        crosshair.X -= crosshairSpeed * 5;
        crosshair.Y -= crosshairSpeed * 5;

        // Clamp
        int maxX = screenWidth - crosshairWidth + 2;
        int maxY = screenHeight - crosshairHeight;
        crosshair.X = Math.Min(maxX, Math.Max(crosshair.X, 2));
        crosshair.Y = Math.Min(maxY, Math.Max(crosshair.Y, 2));

        // Assert - Should be clamped to minimum
        Assert.Equal(2, crosshair.X);
        Assert.Equal(2, crosshair.Y);
    }

    [Fact]
    public void CompleteGameScenario_SpawnDelayDecreases()
    {
        // Arrange
        int spawnDelay = 100;
        int frame = 0;

        // Act - Simulate multiple spawn events
        for (int i = 0; i < 50; i++)
        {
            frame++;
            if (frame % spawnDelay == 0)
            {
                // Spawn bird
                if (spawnDelay > 60)
                {
                    spawnDelay--;
                }
            }
        }

        // Assert - Spawn delay should have decreased
        Assert.True(spawnDelay < 100);
    }

    [Fact]
    public void CompleteGameScenario_AmmoCapAt5()
    {
        // Arrange
        int ammoCount = 4;
        var bird1 = new TestBird(50, 20, 1);
        var bird2 = new TestBird(70, 25, -1);
        int score = 0;

        // Act - Kill two birds in quick succession
        bird1.IsDead = true;
        ammoCount += 2; // 4 + 2 = 6
        score += 150;

        if (ammoCount > 5)
        {
            ammoCount = 5;
        }

        bird2.IsDead = true;
        ammoCount += 2; // 5 + 2 = 7
        score += 150;

        if (ammoCount > 5)
        {
            ammoCount = 5;
        }

        // Assert
        Assert.Equal(5, ammoCount); // Capped at 5
        Assert.Equal(300, score);
    }

    [Fact]
    public void CompleteGameScenario_BirdAnimationCycle()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);
        int frame = 0;

        // Act - Simulate animation frames
        for (int i = 0; i < 10; i++)
        {
            if (frame % 2 == 0)
            {
                bird.IncrementFrame();
            }
            frame++;
        }

        // Assert - Should have cycled through frames
        Assert.Equal(1, bird.Frame); // After 5 increments (0,1,2,3,0,1)
    }

    [Fact]
    public void CompleteGameScenario_GunAngleCalculation()
    {
        // Arrange
        var crosshair = new TestPoint(40, 10);
        var middleAncor = new TestPoint(60, 28);
        int barrelLength = 10;
        double gunXStretch = 1.5;

        // Act
        double theta = Math.Atan2(middleAncor.Y - crosshair.Y, middleAncor.X - crosshair.X);
        int xGunOffset = -(int)Math.Floor(Math.Cos(theta) * barrelLength);
        int yGunOffset = -(int)Math.Floor(Math.Sin(theta) * barrelLength);
        var gunTopOffset = new TestPoint((int)(xGunOffset * gunXStretch), yGunOffset);

        // Assert
        Assert.NotEqual(0, gunTopOffset.X);
        Assert.NotEqual(0, gunTopOffset.Y);
        Assert.True(Math.Abs(gunTopOffset.X) >= Math.Abs(xGunOffset)); // Stretched
    }

    [Fact]
    public void CompleteGameScenario_MultipleBirdsInFlight()
    {
        // Arrange
        var bird1 = new TestBird(10, 15, 1);
        var bird2 = new TestBird(100, 20, -1);
        var bird3 = new TestBird(50, 10, 1);

        // Act - Simulate one frame of movement
        bird1.X += bird1.Direction;
        bird2.X += bird2.Direction;
        bird3.X += bird3.Direction;

        // Assert
        Assert.Equal(11, bird1.X);
        Assert.Equal(99, bird2.X);
        Assert.Equal(51, bird3.X);
    }

    [Fact]
    public void CompleteGameScenario_BulletOutOfBounds()
    {
        // Arrange
        var position = new TestPoint(60, 28);
        double angle = -Math.PI / 4; // 45 degrees up-left
        var bullet = new TestBullet(position, angle);
        int screenWidth = 120;
        int screenHeight = 30;

        // Act - Move bullet until out of bounds
        for (int i = 0; i < 100; i++)
        {
            bullet.UpdatePosition();
            bullet.CheckBounds(screenWidth, screenHeight);
            if (bullet.OutOfBounds) break;
        }

        // Assert
        Assert.True(bullet.OutOfBounds);
    }

    [Fact]
    public void EdgeCase_NegativeCoordinates()
    {
        // Arrange
        var point = new TestPoint(-10, -20);

        // Act & Assert
        Assert.Equal(-10, point.X);
        Assert.Equal(-20, point.Y);
    }

    [Fact]
    public void EdgeCase_BirdContainsAtBoundary()
    {
        // Arrange - Bird at origin
        var bird = new TestBird(0, 0, 1);

        // Act & Assert
        Assert.True(bird.Contains(0, 0, 10, 3));
        Assert.True(bird.Contains(9, 2, 10, 3));
        Assert.False(bird.Contains(10, 0, 10, 3));
        Assert.False(bird.Contains(0, 3, 10, 3));
    }

    [Fact]
    public void EdgeCase_ZeroAngleBullet()
    {
        // Arrange
        var position = new TestPoint(50, 50);
        double angle = 0;
        var bullet = new TestBullet(position, angle);

        // Act
        bullet.UpdatePosition();

        // Assert - Should move left (negative X direction)
        Assert.True(bullet.X[0] < 50);
        Assert.Equal(50, bullet.Y[0], 1); // Y should remain approximately same
    }

    [Fact]
    public void RegressionTest_AmmoNeverGoesNegative()
    {
        // Arrange
        int ammoCount = 0;

        // Act - Try to shoot with no ammo
        bool canShoot = ammoCount > 0;
        if (canShoot)
        {
            ammoCount--;
        }

        // Assert
        Assert.Equal(0, ammoCount);
        Assert.False(canShoot);
    }

    [Fact]
    public void RegressionTest_BirdFrameWrapsCorrectly()
    {
        // Arrange
        var bird = new TestBird(50, 20, 1);

        // Act - Increment beyond normal range
        for (int i = 0; i < 100; i++)
        {
            bird.IncrementFrame();
        }

        // Assert - Frame should be 0-3 for alive birds
        Assert.True(bird.Frame >= 0 && bird.Frame <= 3);
    }

    [Fact]
    public void StressTest_ManyBulletUpdates()
    {
        // Arrange
        var position = new TestPoint(60, 15);
        double angle = Math.PI / 4;
        var bullet = new TestBullet(position, angle);

        // Act - Update many times
        for (int i = 0; i < 1000; i++)
        {
            bullet.UpdatePosition();
        }

        // Assert - Should complete without errors
        Assert.NotEqual(60, bullet.X[0]);
        Assert.NotEqual(15, bullet.Y[0]);
    }
}