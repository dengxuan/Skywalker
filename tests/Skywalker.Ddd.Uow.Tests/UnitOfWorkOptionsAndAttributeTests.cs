using System.Data;
using Skywalker.Ddd.Uow;

namespace Skywalker.Ddd.Uow.Tests;

public class UnitOfWorkOptionsTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveDefaults()
    {
        var options = new UnitOfWorkOptions();
        Assert.False(options.IsTransactional);
        Assert.Null(options.IsolationLevel);
        Assert.Null(options.Timeout);
    }

    [Fact]
    public void ParameterizedConstructor_ShouldSetProperties()
    {
        var options = new UnitOfWorkOptions(true, IsolationLevel.Serializable, 5000);
        Assert.True(options.IsTransactional);
        Assert.Equal(IsolationLevel.Serializable, options.IsolationLevel);
        Assert.Equal(5000, options.Timeout);
    }

    [Fact]
    public void Clone_ShouldCopyAllProperties()
    {
        var original = new UnitOfWorkOptions(true, IsolationLevel.ReadCommitted, 3000);
        var clone = original.Clone();

        Assert.True(clone.IsTransactional);
        Assert.Equal(IsolationLevel.ReadCommitted, clone.IsolationLevel);
        Assert.Equal(3000, clone.Timeout);
    }

    [Fact]
    public void Clone_ShouldBeIndependent()
    {
        var original = new UnitOfWorkOptions(true);
        var clone = original.Clone();
        clone.IsTransactional = false;

        Assert.True(original.IsTransactional);
        Assert.False(clone.IsTransactional);
    }
}

public class UnitOfWorkAttributeTests
{
    [Fact]
    public void DefaultConstructor_ShouldHaveNullProperties()
    {
        var attr = new UnitOfWorkAttribute();
        Assert.Null(attr.IsTransactional);
        Assert.Null(attr.Timeout);
        Assert.Null(attr.IsolationLevel);
        Assert.False(attr.IsDisabled);
    }

    [Fact]
    public void Constructor_WithTransactional_ShouldSet()
    {
        var attr = new UnitOfWorkAttribute(true);
        Assert.True(attr.IsTransactional);
    }

    [Fact]
    public void Constructor_WithTransactionalAndIsolation_ShouldSet()
    {
        var attr = new UnitOfWorkAttribute(true, IsolationLevel.Snapshot);
        Assert.True(attr.IsTransactional);
        Assert.Equal(IsolationLevel.Snapshot, attr.IsolationLevel);
    }

    [Fact]
    public void Constructor_WithAllParams_ShouldSet()
    {
        var attr = new UnitOfWorkAttribute(true, IsolationLevel.Serializable, 5000);
        Assert.True(attr.IsTransactional);
        Assert.Equal(IsolationLevel.Serializable, attr.IsolationLevel);
        Assert.Equal(5000, attr.Timeout);
    }

    [Fact]
    public void SetOptions_ShouldCopyOnlySetValues()
    {
        var attr = new UnitOfWorkAttribute { IsTransactional = true };
        var options = new UnitOfWorkOptions();

        attr.SetOptions(options);
        Assert.True(options.IsTransactional);
        Assert.Null(options.IsolationLevel);
        Assert.Null(options.Timeout);
    }

    [Fact]
    public void SetOptions_WithAllValues_ShouldCopyAll()
    {
        var attr = new UnitOfWorkAttribute
        {
            IsTransactional = true,
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = 3000
        };
        var options = new UnitOfWorkOptions();

        attr.SetOptions(options);
        Assert.True(options.IsTransactional);
        Assert.Equal(IsolationLevel.ReadCommitted, options.IsolationLevel);
        Assert.Equal(3000, options.Timeout);
    }

    [Fact]
    public void SetOptions_WithNoValues_ShouldNotChange()
    {
        var attr = new UnitOfWorkAttribute();
        var options = new UnitOfWorkOptions(true, IsolationLevel.Serializable, 5000);

        attr.SetOptions(options);
        Assert.True(options.IsTransactional);
        Assert.Equal(IsolationLevel.Serializable, options.IsolationLevel);
        Assert.Equal(5000, options.Timeout);
    }
}
