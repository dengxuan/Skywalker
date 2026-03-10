using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.EventBus.Abstractions;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Ddd.EntityFrameworkCore.Tests;

public class RepositoryTests : IDisposable
{
    private readonly TestDbContext _dbContext;
    private readonly Repository<TestDbContext, TestEntity, Guid> _repository;
    private readonly IClock _clock;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWorkAccessor _unitOfWorkAccessor;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TestDbContext(options);
        _clock = Substitute.For<IClock>();
        _clock.Now.Returns(DateTime.UtcNow);
        _eventBus = Substitute.For<IEventBus>();
        _unitOfWorkAccessor = Substitute.For<IUnitOfWorkAccessor>();

        var dbContextProvider = Substitute.For<IDbContextProvider<TestDbContext>>();
        dbContextProvider.GetDbContext().Returns(_dbContext);

        _repository = new Repository<TestDbContext, TestEntity, Guid>(_clock, _eventBus, dbContextProvider, _unitOfWorkAccessor);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task InsertAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid()) { Name = "Test", Value = 42 };

        // Act
        var result = await _repository.InsertAsync(entity, autoSave: true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);

        var fromDb = await _dbContext.TestEntities.FindAsync(entity.Id);
        Assert.NotNull(fromDb);
        Assert.Equal("Test", fromDb!.Name);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid()) { Name = "FindTest", Value = 100 };
        _dbContext.TestEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("FindTest", result!.Name);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.FindAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid()) { Name = "Original", Value = 1 };
        _dbContext.TestEntities.Add(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        entity.Name = "Updated";
        await _repository.UpdateAsync(entity, autoSave: true);

        // Assert
        var fromDb = await _dbContext.TestEntities.FindAsync(entity.Id);
        Assert.Equal("Updated", fromDb!.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new TestEntity(Guid.NewGuid()) { Name = "ToDelete", Value = 0 };
        _dbContext.TestEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity, autoSave: true);

        // Assert
        var fromDb = await _dbContext.TestEntities.FindAsync(entity.Id);
        Assert.Null(fromDb);
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnAllEntities()
    {
        // Arrange
        _dbContext.TestEntities.AddRange(
            new TestEntity(Guid.NewGuid()) { Name = "Entity1" },
            new TestEntity(Guid.NewGuid()) { Name = "Entity2" },
            new TestEntity(Guid.NewGuid()) { Name = "Entity3" }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetListAsync();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        _dbContext.TestEntities.AddRange(
            new TestEntity(Guid.NewGuid()) { Name = "A" },
            new TestEntity(Guid.NewGuid()) { Name = "B" }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountAsync_WithFilter_ShouldReturnFilteredCount()
    {
        // Arrange
        _dbContext.TestEntities.AddRange(
            new TestEntity(Guid.NewGuid()) { Name = "Match", Value = 10 },
            new TestEntity(Guid.NewGuid()) { Name = "Match", Value = 20 },
            new TestEntity(Guid.NewGuid()) { Name = "NoMatch", Value = 30 }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync(e => e.Name == "Match");

        // Assert
        Assert.Equal(2, count);
    }
}

