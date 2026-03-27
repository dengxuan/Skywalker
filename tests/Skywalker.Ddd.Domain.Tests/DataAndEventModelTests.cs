using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Events;
using Skywalker.Ddd.Exceptions;

namespace Skywalker.Ddd.Domain.Tests;

public class DataAndEventModelTests
{
    // ConnectionStrings
    [Fact]
    public void ConnectionStrings_Default_GetSet()
    {
        var cs = new ConnectionStrings();
        cs.Default = "Server=localhost";
        Assert.Equal("Server=localhost", cs.Default);
        Assert.Equal("Server=localhost", cs[ConnectionStrings.DefaultConnectionStringName]);
    }

    [Fact]
    public void ConnectionStrings_InheritsFromDictionary()
    {
        var cs = new ConnectionStrings();
        cs["Custom"] = "CustomConnStr";
        Assert.Equal("CustomConnStr", cs["Custom"]);
    }

    // ConnectionStringNameAttribute
    [Fact]
    public void ConnectionStringNameAttribute_ReturnsName()
    {
        var name = ConnectionStringNameAttribute.GetConnectionStringName<TestDbClass>();
        Assert.Equal("TestDb", name);
    }

    [Fact]
    public void ConnectionStringNameAttribute_ReturnsFullName_WhenNoAttribute()
    {
        var name = ConnectionStringNameAttribute.GetConnectionStringName<NoAttributeClass>();
        Assert.Equal(typeof(NoAttributeClass).FullName, name);
    }

    // SkywalkerDbConnectionOptions
    [Fact]
    public void SkywalkerDbConnectionOptions_HasDefaultConnectionStrings()
    {
        var options = new SkywalkerDbConnectionOptions();
        Assert.NotNull(options.ConnectionStrings);
    }

    // DataSeedContext
    [Fact]
    public void DataSeedContext_Properties_GetSet()
    {
        var ctx = new DataSeedContext();
        ctx["key1"] = "value1";
        Assert.Equal("value1", ctx["key1"]);
    }

    [Fact]
    public void DataSeedContext_WithProperty_ReturnsSelf()
    {
        var ctx = new DataSeedContext();
        var result = ctx.WithProperty("key", "value");
        Assert.Same(ctx, result);
        Assert.Equal("value", ctx["key"]);
    }

    [Fact]
    public void DataSeedContext_MissingKey_ReturnsNull()
    {
        var ctx = new DataSeedContext();
        Assert.Null(ctx["nonexistent"]);
    }

    // SkywalkerDbConcurrencyException
    [Fact]
    public void SkywalkerDbConcurrencyException_DefaultConstructor()
    {
        var ex = new SkywalkerDbConcurrencyException();
        Assert.NotNull(ex);
    }

    [Fact]
    public void SkywalkerDbConcurrencyException_WithMessage()
    {
        var ex = new SkywalkerDbConcurrencyException("conflict");
        Assert.Equal("conflict", ex.Message);
    }

    [Fact]
    public void SkywalkerDbConcurrencyException_WithInnerException()
    {
        var inner = new Exception("inner");
        var ex = new SkywalkerDbConcurrencyException("conflict", inner);
        Assert.Same(inner, ex.InnerException);
    }

    // EntityAlreadyExistedException
    [Fact]
    public void EntityAlreadyExistedException_DefaultConstructor()
    {
        var ex = new EntityAlreadyExistedException();
        Assert.NotNull(ex);
    }

    [Fact]
    public void EntityAlreadyExistedException_WithType()
    {
        var ex = new EntityAlreadyExistedException(typeof(string));
        Assert.Equal(typeof(string), ex.EntityType);
    }

    [Fact]
    public void EntityAlreadyExistedException_WithTypeAndId()
    {
        var ex = new EntityAlreadyExistedException(typeof(string), 42);
        Assert.Equal(typeof(string), ex.EntityType);
        Assert.Equal(42, ex.Id);
        Assert.Contains("42", ex.Message);
    }

    [Fact]
    public void EntityAlreadyExistedException_WithMessage()
    {
        var ex = new EntityAlreadyExistedException("custom");
        Assert.Equal("custom", ex.Message);
    }

    [Fact]
    public void EntityAlreadyExistedException_WithMessageAndInner()
    {
        var inner = new Exception();
        var ex = new EntityAlreadyExistedException("custom", inner);
        Assert.Same(inner, ex.InnerException);
    }

    // DomainEventEntry
    [Fact]
    public void DomainEventEntry_StoresValues()
    {
        var entity = new object();
        var eventData = new object();
        var entry = new DomainEventEntry(entity, eventData);
        Assert.Same(entity, entry.SourceEntity);
        Assert.Same(eventData, entry.EventData);
    }

    // EntityChangeEntry
    [Fact]
    public void EntityChangeEntry_StoresValues()
    {
        var entity = new object();
        var entry = new EntityChangeEntry(entity, EntityChangeType.Created);
        Assert.Same(entity, entry.Entity);
        Assert.Equal(EntityChangeType.Created, entry.ChangeType);
    }

    // ExceptionNotificationContext
    [Fact]
    public void ExceptionNotificationContext_DefaultLogLevel()
    {
        var ex = new InvalidOperationException("test");
        var ctx = new ExceptionNotificationContext(ex);
        Assert.Same(ex, ctx.Exception);
        Assert.Equal(LogLevel.Error, ctx.LogLevel);
        Assert.True(ctx.Handled);
    }

    [Fact]
    public void ExceptionNotificationContext_CustomLogLevel()
    {
        var ex = new InvalidOperationException("test");
        var ctx = new ExceptionNotificationContext(ex, LogLevel.Warning, false);
        Assert.Equal(LogLevel.Warning, ctx.LogLevel);
        Assert.False(ctx.Handled);
    }

    [ConnectionStringName("TestDb")]
    private class TestDbClass { }

    private class NoAttributeClass { }
}
