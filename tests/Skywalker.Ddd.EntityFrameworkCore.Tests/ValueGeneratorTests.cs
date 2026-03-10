using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Skywalker.Ddd.EntityFrameworkCore.ValueGeneration;

namespace Skywalker.Ddd.EntityFrameworkCore.Tests;

public class ValueGeneratorTests
{
    [Fact]
    public void GuidIdValueGenerator_ShouldGenerateNonEmptyGuid()
    {
        // Arrange
        var generator = new GuidIdValueGenerator();
        var entry = CreateMockEntityEntry();

        // Act
        var result = generator.Next(entry);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public void GuidIdValueGenerator_ShouldGenerateUniqueGuids()
    {
        // Arrange
        var generator = new GuidIdValueGenerator();
        var entry = CreateMockEntityEntry();

        // Act
        var guids = Enumerable.Range(0, 100)
            .Select(_ => generator.Next(entry))
            .ToList();

        // Assert
        Assert.Equal(100, guids.Distinct().Count());
    }

    [Fact]
    public void GuidIdValueGenerator_GeneratesTemporaryValues_ShouldBeFalse()
    {
        // Arrange
        var generator = new GuidIdValueGenerator();

        // Assert
        Assert.False(generator.GeneratesTemporaryValues);
    }

    [Fact]
    public void StringIdValueGenerator_ShouldGenerateNonEmptyString()
    {
        // Arrange
        var generator = new StringIdValueGenerator();
        var entry = CreateMockEntityEntry();

        // Act
        var result = generator.Next(entry);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void StringIdValueGenerator_ShouldContainDateTimeFormat()
    {
        // Arrange
        var generator = new StringIdValueGenerator();
        var entry = CreateMockEntityEntry();

        // Act
        var result = generator.Next(entry);

        // Assert
        // 格式: yyyyMMddHHmmssffffff-{correlationId}
        Assert.Contains("-", result);
        var datePart = result.Split('-')[0];
        Assert.True(datePart.Length >= 18 && datePart.Length <= 20, $"Date part length should be between 18 and 20, but was {datePart.Length}");
    }

    [Fact]
    public void StringIdValueGenerator_ShouldGenerateUniqueIds()
    {
        // Arrange
        var generator = new StringIdValueGenerator();
        var entry = CreateMockEntityEntry();

        // Act
        var ids = Enumerable.Range(0, 100)
            .Select(_ => generator.Next(entry))
            .ToList();

        // Assert
        Assert.Equal(100, ids.Distinct().Count());
    }

    [Fact]
    public void StringIdValueGenerator_GeneratesTemporaryValues_ShouldBeFalse()
    {
        // Arrange
        var generator = new StringIdValueGenerator();

        // Assert
        Assert.False(generator.GeneratesTemporaryValues);
    }

    private static EntityEntry CreateMockEntityEntry()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TestEntity(Guid.NewGuid());
        return context.Entry(entity);
    }
}

