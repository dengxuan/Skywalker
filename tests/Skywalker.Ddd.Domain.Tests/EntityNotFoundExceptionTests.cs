using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Tests;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void DefaultConstructor_CreatesException()
    {
        var ex = new EntityNotFoundException();
        Assert.Null(ex.EntityType);
        Assert.Null(ex.Id);
    }

    [Fact]
    public void Constructor_WithEntityType_SetsEntityType()
    {
        var ex = new EntityNotFoundException(typeof(string));
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Null(ex.Id);
    }

    [Fact]
    public void Constructor_WithEntityTypeAndId_SetsBoth()
    {
        var ex = new EntityNotFoundException(typeof(string), 42);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Equal(42, ex.Id);
        Assert.Contains("String", ex.Message);
        Assert.Contains("42", ex.Message);
    }

    [Fact]
    public void Constructor_WithEntityTypeAndNullId_MessageWithoutId()
    {
        var ex = new EntityNotFoundException(typeof(string), null);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Null(ex.Id);
        Assert.Contains("String", ex.Message);
    }

    [Fact]
    public void Constructor_WithEntityTypeIdAndInnerException()
    {
        var inner = new InvalidOperationException("inner error");
        var ex = new EntityNotFoundException(typeof(string), 42, inner);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Equal(42, ex.Id);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void Constructor_WithMessage()
    {
        var ex = new EntityNotFoundException("custom message");
        Assert.Equal("custom message", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new EntityNotFoundException("custom message", inner);
        Assert.Equal("custom message", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }
}
