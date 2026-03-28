using System.ComponentModel.DataAnnotations;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Values;

namespace Skywalker.Ddd.Domain.Tests;

#region Test Entities

public class TestEntity : Entity<int>
{
    public string Name { get; set; } = string.Empty;

    public TestEntity() : base() { }
    public TestEntity(int id) : base(id) { Name = $"Entity-{id}"; }
}

public class TestEntity2 : Entity<int>
{
    public TestEntity2(int id) : base(id) { }
}

public class TestAggregateRoot : AggregateRoot<int>
{
    public TestAggregateRoot() : base() { }
    public TestAggregateRoot(int id) : base(id) { }

    public void RaiseEvent(object evt) => AddDistributedEvent(evt);
}

public class TestValueObject : ValueObject
{
    public string Street { get; }
    public string City { get; }

    public TestValueObject(string street, string city)
    {
        Street = street;
        City = city;
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Street;
        yield return City;
    }
}

public class CompositeKeyEntity : Entity
{
    public string Key1 { get; set; } = string.Empty;
    public int Key2 { get; set; }

    public override object[] GetKeys() => new object[] { Key1, Key2 };
}

#endregion

public class EntityTests
{
    [Fact]
    public void Entity_ShouldHaveConcurrencyStamp()
    {
        var entity = new TestEntity(1);
        Assert.NotNull(entity.ConcurrencyStamp);
        Assert.NotEmpty(entity.ConcurrencyStamp);
    }

    [Fact]
    public void Entity_GetKeys_ShouldReturnId()
    {
        var entity = new TestEntity(42);
        var keys = entity.GetKeys();
        Assert.Single(keys);
        Assert.Equal(42, keys[0]);
    }

    [Fact]
    public void Entity_ToString_ShouldContainId()
    {
        var entity = new TestEntity(42);
        var str = entity.ToString();
        Assert.Contains("42", str);
        Assert.Contains("TestEntity", str);
    }

    [Fact]
    public void Entity_EqualsKey_ShouldCompareById()
    {
        var entity = new TestEntity(1);
        Assert.True(entity.Equals(1));
        Assert.False(entity.Equals(2));
    }

    [Fact]
    public void Entity_EntityEquals_SameId_ShouldBeEqual()
    {
        var e1 = new TestEntity(1);
        var e2 = new TestEntity(1);
        Assert.True(e1.EntityEquals(e2));
    }

    [Fact]
    public void Entity_EntityEquals_DifferentId_ShouldNotBeEqual()
    {
        var e1 = new TestEntity(1);
        var e2 = new TestEntity(2);
        Assert.False(e1.EntityEquals(e2));
    }

    [Fact]
    public void Entity_EntityEquals_SameReference_ShouldBeEqual()
    {
        var e1 = new TestEntity(1);
        Assert.True(e1.EntityEquals(e1));
    }

    [Fact]
    public void Entity_EntityEquals_Null_ShouldNotBeEqual()
    {
        var e1 = new TestEntity(1);
        Assert.False(e1.EntityEquals(null!));
    }

    [Fact]
    public void Entity_EntityEquals_DefaultKeys_ShouldNotBeEqual()
    {
        var e1 = new TestEntity(0);
        var e2 = new TestEntity(0);
        Assert.False(e1.EntityEquals(e2));
    }

    [Fact]
    public void Entity_DefaultConstructor_ShouldSetDefaultId()
    {
        var entity = new TestEntity();
        Assert.Equal(0, entity.Id);
    }
}

public class AggregateRootTests
{
    [Fact]
    public void AggregateRoot_ShouldHaveConcurrencyStamp()
    {
        var root = new TestAggregateRoot(1);
        Assert.NotNull(root.ConcurrencyStamp);
    }

    [Fact]
    public void AggregateRoot_DistributedEvents_ShouldBeEmpty()
    {
        var root = new TestAggregateRoot(1);
        Assert.Empty(root.GetDistributedEvents());
    }

    [Fact]
    public void AggregateRoot_AddAndGetDistributedEvents_ShouldWork()
    {
        var root = new TestAggregateRoot(1);
        root.RaiseEvent("event1");
        root.RaiseEvent("event2");
        var events = root.GetDistributedEvents().ToList();
        Assert.Equal(2, events.Count);
        Assert.Equal("event1", events[0]);
        Assert.Equal("event2", events[1]);
    }

    [Fact]
    public void AggregateRoot_ClearDistributedEvents_ShouldClear()
    {
        var root = new TestAggregateRoot(1);
        root.RaiseEvent("event1");
        root.ClearDistributedEvents();
        Assert.Empty(root.GetDistributedEvents());
    }

    [Fact]
    public void AggregateRoot_Validate_ShouldReturnEmpty()
    {
        var root = new TestAggregateRoot(1);
        var context = new ValidationContext(root);
        var errors = root.Validate(context);
        Assert.Empty(errors);
    }

    [Fact]
    public void AggregateRoot_DefaultConstructor_ShouldWork()
    {
        var root = new TestAggregateRoot();
        Assert.Equal(0, root.Id);
    }
}

public class ValueObjectTests
{
    [Fact]
    public void ValueEquals_SameValues_ShouldBeEqual()
    {
        var addr1 = new TestValueObject("123 Main", "Springfield");
        var addr2 = new TestValueObject("123 Main", "Springfield");
        Assert.True(addr1.ValueEquals(addr2));
    }

    [Fact]
    public void ValueEquals_DifferentValues_ShouldNotBeEqual()
    {
        var addr1 = new TestValueObject("123 Main", "Springfield");
        var addr2 = new TestValueObject("456 Oak", "Springfield");
        Assert.False(addr1.ValueEquals(addr2));
    }

    [Fact]
    public void ValueEquals_Null_ShouldNotBeEqual()
    {
        var addr = new TestValueObject("123 Main", "Springfield");
        Assert.False(addr.ValueEquals(null));
    }

    [Fact]
    public void ValueEquals_DifferentType_ShouldNotBeEqual()
    {
        var addr = new TestValueObject("123 Main", "Springfield");
        Assert.False(addr.ValueEquals("not a value object"));
    }

    [Fact]
    public void ValueEquals_SameReference_ShouldBeEqual()
    {
        var addr = new TestValueObject("123 Main", "Springfield");
        Assert.True(addr.ValueEquals(addr));
    }
}

public class EntityHelperTests
{
    [Fact]
    public void EntityEquals_BothNull_ShouldReturnFalse()
    {
        Assert.False(EntityHelper.EntityEquals(null, null));
    }

    [Fact]
    public void EntityEquals_OneNull_ShouldReturnFalse()
    {
        var entity = new TestEntity(1);
        Assert.False(EntityHelper.EntityEquals(entity, null));
        Assert.False(EntityHelper.EntityEquals(null, entity));
    }

    [Fact]
    public void EntityEquals_SameReference_ShouldReturnTrue()
    {
        var entity = new TestEntity(1);
        Assert.True(EntityHelper.EntityEquals(entity, entity));
    }

    [Fact]
    public void EntityEquals_SameId_ShouldReturnTrue()
    {
        Assert.True(EntityHelper.EntityEquals(new TestEntity(1), new TestEntity(1)));
    }

    [Fact]
    public void EntityEquals_DifferentId_ShouldReturnFalse()
    {
        Assert.False(EntityHelper.EntityEquals(new TestEntity(1), new TestEntity(2)));
    }

    [Fact]
    public void EntityEquals_IncompatibleTypes_ShouldReturnFalse()
    {
        var e1 = new TestEntity(1);
        var e2 = new TestEntity2(1);
        Assert.False(EntityHelper.EntityEquals(e1, e2));
    }

    [Fact]
    public void IsValueObject_WithValueObjectType_ShouldReturnTrue()
    {
        Assert.True(EntityHelper.IsValueObject(typeof(TestValueObject)));
    }

    [Fact]
    public void IsValueObject_WithNonValueObjectType_ShouldReturnFalse()
    {
        Assert.False(EntityHelper.IsValueObject(typeof(string)));
    }

    [Fact]
    public void IsValueObject_WithObject_ShouldWork()
    {
        var vo = new TestValueObject("a", "b");
        Assert.True(EntityHelper.IsValueObject(vo));
        Assert.False(EntityHelper.IsValueObject((object?)null));
    }

    [Fact]
    public void IsEntity_WithEntityType_ShouldReturnTrue()
    {
        Assert.True(EntityHelper.IsEntity(typeof(TestEntity)));
    }

    [Fact]
    public void IsEntity_WithNonEntityType_ShouldReturnFalse()
    {
        Assert.False(EntityHelper.IsEntity(typeof(string)));
    }

    [Fact]
    public void CheckEntity_WithNonEntity_ShouldThrow()
    {
        Assert.Throws<Skywalker.Exceptions.SkywalkerException>(() => EntityHelper.CheckEntity(typeof(string)));
    }

    [Fact]
    public void CheckEntity_WithEntity_ShouldNotThrow()
    {
        EntityHelper.CheckEntity(typeof(TestEntity));
    }

    [Fact]
    public void IsEntityWithId_ShouldReturnTrueForGenericEntity()
    {
        Assert.True(EntityHelper.IsEntityWithId(typeof(TestEntity)));
    }

    [Fact]
    public void IsEntityWithId_ShouldReturnFalseForCompositeKey()
    {
        Assert.False(EntityHelper.IsEntityWithId(typeof(CompositeKeyEntity)));
    }

    [Fact]
    public void HasDefaultId_WithDefault_ShouldReturnTrue()
    {
        var entity = new TestEntity(0);
        Assert.True(EntityHelper.HasDefaultId(entity));
    }

    [Fact]
    public void HasDefaultId_WithValue_ShouldReturnFalse()
    {
        var entity = new TestEntity(42);
        Assert.False(EntityHelper.HasDefaultId(entity));
    }

    [Fact]
    public void HasDefaultKeys_WithDefault_ShouldReturnTrue()
    {
        var entity = new TestEntity(0);
        Assert.True(EntityHelper.HasDefaultKeys(entity));
    }

    [Fact]
    public void HasDefaultKeys_WithValue_ShouldReturnFalse()
    {
        var entity = new TestEntity(42);
        Assert.False(EntityHelper.HasDefaultKeys(entity));
    }

    [Fact]
    public void FindPrimaryKeyType_WithGenericEntity_ShouldReturnKeyType()
    {
        Assert.Equal(typeof(int), EntityHelper.FindPrimaryKeyType<TestEntity>());
    }

    [Fact]
    public void FindPrimaryKeyType_WithCompositeKey_ShouldReturnNull()
    {
        Assert.Null(EntityHelper.FindPrimaryKeyType(typeof(CompositeKeyEntity)));
    }

    [Fact]
    public void FindPrimaryKeyType_WithNonEntity_ShouldThrow()
    {
        Assert.Throws<Skywalker.Exceptions.SkywalkerException>(
            () => EntityHelper.FindPrimaryKeyType(typeof(string)));
    }

    [Fact]
    public void CreateEqualityExpressionForId_ShouldMatchEntity()
    {
        var expr = EntityHelper.CreateEqualityExpressionForId<TestEntity, int>(42);
        var compiled = expr.Compile();
        Assert.True(compiled(new TestEntity(42)));
        Assert.False(compiled(new TestEntity(1)));
    }
}
