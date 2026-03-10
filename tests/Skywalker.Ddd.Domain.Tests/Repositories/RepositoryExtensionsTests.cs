using System.Linq.Expressions;
using NSubstitute;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Extensions.Specifications;

namespace Skywalker.Ddd.Domain.Tests.Repositories;

public class RepositoryExtensionsTests
{
    private readonly IBasicRepository<TestEntity> _basicRepository;
    private readonly IBasicRepository<TestEntity, Guid> _basicRepositoryWithKey;
    private readonly IReadOnlyRepository<TestEntity> _readOnlyRepository;

    public RepositoryExtensionsTests()
    {
        _basicRepository = Substitute.For<IBasicRepository<TestEntity>>();
        _basicRepositoryWithKey = Substitute.For<IBasicRepository<TestEntity, Guid>>();
        _readOnlyRepository = Substitute.For<IReadOnlyRepository<TestEntity>>();
    }



    [Fact]
    public async Task InsertManyAsync_ShouldReturnCount()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new(Guid.NewGuid(), "Test1"),
            new(Guid.NewGuid(), "Test2")
        };
        _basicRepository.InsertAsync(Arg.Any<IEnumerable<TestEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(2);

        // Act
        var result = await _basicRepository.InsertManyAsync(entities, true);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task UpdateManyAsync_ShouldUpdateAllEntities()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new(Guid.NewGuid(), "Test1"),
            new(Guid.NewGuid(), "Test2"),
            new(Guid.NewGuid(), "Test3")
        };

        // Act
        var result = await _basicRepository.UpdateManyAsync(entities);

        // Assert
        Assert.Equal(3, result);
        await _basicRepository.Received(3).UpdateAsync(
            Arg.Any<TestEntity>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteManyAsync_ShouldDeleteAllEntities()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new(Guid.NewGuid(), "Test1"),
            new(Guid.NewGuid(), "Test2")
        };

        // Act
        var result = await _basicRepository.DeleteManyAsync(entities);

        // Assert
        Assert.Equal(2, result);
        await _basicRepository.Received(2).DeleteAsync(
            Arg.Any<TestEntity>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteManyAsync_ByIds_ShouldDeleteAllById()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await _basicRepositoryWithKey.DeleteManyAsync(ids);

        // Assert
        Assert.Equal(3, result);
        await _basicRepositoryWithKey.Received(3).DeleteAsync(
            Arg.Any<Guid>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithFilter_ShouldCallFind()
    {
        // Arrange
        var expected = new TestEntity(Guid.NewGuid(), "Test");
        _readOnlyRepository.FindAsync(Arg.Any<Expression<Func<TestEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        Expression<Func<TestEntity, bool>> filter = e => e.Name == "Test";
        var result = await _readOnlyRepository.FirstOrDefaultAsync(filter);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSpecification_UsesImplicitConversion()
    {
        // Arrange
        var expected = new TestEntity(Guid.NewGuid(), "Test");
        _readOnlyRepository.FindAsync(Arg.Any<Expression<Func<TestEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act - Specification<T> implicitly converts to Expression<Func<T, bool>>
        var specification = new TestSpecification("Test");
        var result = await _readOnlyRepository.FirstOrDefaultAsync(specification);

        // Assert
        Assert.Equal(expected, result);
    }
}

