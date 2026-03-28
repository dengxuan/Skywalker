using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Events;
using Skywalker.Ddd.Domain.Events.Distributed;

namespace Skywalker.Ddd.Domain.Tests;

// Reuses TestEntity from EntityTests.cs

public class EntityEventDataTests
{
    [Fact]
    public void EntityEventData_StoresEntity()
    {
        var entity = new TestEntity(1);
        var eventData = new EntityCreatedEventData<TestEntity>(entity);

        Assert.Same(entity, eventData.Entity);
    }

    [Fact]
    public void EntityEventData_GetConstructorArgs_ReturnsEntity()
    {
        var entity = new TestEntity(1);
        var eventData = new EntityCreatedEventData<TestEntity>(entity);

        var args = eventData.GetConstructorArgs();
        Assert.Single(args);
        Assert.Same(entity, args[0]);
    }

    [Fact]
    public void EntityCreatedEventData_IsEntityChangedEventData()
    {
        var entity = new TestEntity(1);
        var eventData = new EntityCreatedEventData<TestEntity>(entity);

        Assert.IsAssignableFrom<EntityChangedEventData<TestEntity>>(eventData);
        Assert.IsAssignableFrom<EntityEventData<TestEntity>>(eventData);
    }

    [Fact]
    public void EntityUpdatedEventData_IsEntityChangedEventData()
    {
        var entity = new TestEntity(2);
        var eventData = new EntityUpdatedEventData<TestEntity>(entity);

        Assert.IsAssignableFrom<EntityChangedEventData<TestEntity>>(eventData);
    }

    [Fact]
    public void EntityDeletedEventData_IsEntityChangedEventData()
    {
        var entity = new TestEntity(3);
        var eventData = new EntityDeletedEventData<TestEntity>(entity);

        Assert.IsAssignableFrom<EntityChangedEventData<TestEntity>>(eventData);
    }

    [Fact]
    public void EntityChangedEventData_ExtendsEntityEventData()
    {
        var entity = new TestEntity(1);
        var eventData = new EntityChangedEventData<TestEntity>(entity);

        Assert.IsAssignableFrom<EntityEventData<TestEntity>>(eventData);
    }
}

public class EntityCreatingUpdatingDeletingEventDataTests
{
    [Fact]
    public void EntityCreatingEventData_StoresEntity()
    {
        var entity = new TestEntity(1);
        var eventData = new EntityCreatingEventData<TestEntity>(entity);

        Assert.Same(entity, eventData.Entity);
    }

    [Fact]
    public void EntityUpdatingEventData_StoresEntity()
    {
        var entity = new TestEntity(2);
        var eventData = new EntityUpdatingEventData<TestEntity>(entity);

        Assert.Same(entity, eventData.Entity);
    }

    [Fact]
    public void EntityDeletingEventData_StoresEntity()
    {
        var entity = new TestEntity(3);
        var eventData = new EntityDeletingEventData<TestEntity>(entity);

        Assert.Same(entity, eventData.Entity);
    }
}

public class DistributedEtoTests
{
    [Fact]
    public void EntityCreatedEto_StoresEntity()
    {
        var entity = new TestEntity(1);
        var eto = new EntityCreatedEto<TestEntity>(entity);

        Assert.Same(entity, eto.Entity);
    }

    [Fact]
    public void EntityUpdatedEto_StoresEntity()
    {
        var entity = new TestEntity(2);
        var eto = new EntityUpdatedEto<TestEntity>(entity);

        Assert.Same(entity, eto.Entity);
    }

    [Fact]
    public void EntityDeletedEto_StoresEntity()
    {
        var entity = new TestEntity(3);
        var eto = new EntityDeletedEto<TestEntity>(entity);

        Assert.Same(entity, eto.Entity);
    }

    [Fact]
    public void EntityEto_Properties()
    {
        var eto = new EntityEto("TestEntity", "1");

        Assert.Equal("TestEntity", eto.EntityType);
        Assert.Equal("1", eto.KeysAsString);
    }

    [Fact]
    public void EntityEto_InheritsPropertiesDictionary()
    {
        var eto = new EntityEto("TestEntity", "1");
        Assert.NotNull(eto.Properties);
        Assert.Empty(eto.Properties);
    }

    [Fact]
    public void EtoMappingDictionary_Add_AddsMapping()
    {
        var dict = new EtoMappingDictionary();
        dict.Add<TestEntity, EntityEto>();

        Assert.True(dict.ContainsKey(typeof(TestEntity)));
        Assert.Equal(typeof(EntityEto), dict[typeof(TestEntity)].EtoType);
    }

    [Fact]
    public void EtoMappingDictionaryItem_Properties()
    {
        var item = new EtoMappingDictionaryItem(typeof(EntityEto), typeof(string));

        Assert.Equal(typeof(EntityEto), item.EtoType);
        Assert.Equal(typeof(string), item.ObjectMappingContextType);
    }

    [Fact]
    public void EtoMappingDictionaryItem_NullMappingContext()
    {
        var item = new EtoMappingDictionaryItem(typeof(EntityEto));

        Assert.Equal(typeof(EntityEto), item.EtoType);
        Assert.Null(item.ObjectMappingContextType);
    }
}

public class DataFilterStateTests
{
    [Fact]
    public void DataFilterState_IsEnabled_False()
    {
        var state = new DataFilterState(false);
        Assert.False(state.IsEnabled);
    }

    [Fact]
    public void DataFilterState_IsEnabled_True()
    {
        var state = new DataFilterState(true);
        Assert.True(state.IsEnabled);
    }

    [Fact]
    public void DataFilterState_Clone_CreatesIndependentCopy()
    {
        var state = new DataFilterState(true);
        var clone = state.Clone();

        Assert.True(clone.IsEnabled);

        clone.IsEnabled = false;
        Assert.True(state.IsEnabled);
        Assert.False(clone.IsEnabled);
    }
}

public class SkywalkerCommonDbPropertiesTests
{
    [Fact]
    public void DbTablePrefix_DefaultIsSky()
    {
        Assert.Equal("Sky", SkywalkerCommonDbProperties.DbTablePrefix);
    }

    [Fact]
    public void DbSchema_DefaultIsNull()
    {
        Assert.Null(SkywalkerCommonDbProperties.DbSchema);
    }
}

public class SkywalkerDataFilterOptionsTests
{
    [Fact]
    public void DefaultStates_InitializedAsEmptyDictionary()
    {
        var options = new SkywalkerDataFilterOptions();
        Assert.NotNull(options.DefaultStates);
        Assert.Empty(options.DefaultStates);
    }

    [Fact]
    public void DefaultStates_CanAddStates()
    {
        var options = new SkywalkerDataFilterOptions();
        options.DefaultStates[typeof(string)] = new DataFilterState(true);

        Assert.Single(options.DefaultStates);
        Assert.True(options.DefaultStates[typeof(string)].IsEnabled);
    }
}

public class SkywalkerDistributedEntityEventOptionsTests
{
    [Fact]
    public void AutoEventSelectors_InitializedAsList()
    {
        var options = new SkywalkerDistributedEntityEventOptions();
        Assert.NotNull(options.AutoEventSelectors);
    }

    [Fact]
    public void EtoMappings_InitializedAsDictionary()
    {
        var options = new SkywalkerDistributedEntityEventOptions();
        Assert.NotNull(options.EtoMappings);
    }
}

public class EntityChangeReportTests
{
    [Fact]
    public void EntityChangeReport_DefaultLists()
    {
        var report = new EntityChangeReport();
        Assert.NotNull(report.ChangedEntities);
        Assert.NotNull(report.DomainEvents);
        Assert.NotNull(report.DistributedEvents);
        Assert.Empty(report.ChangedEntities);
        Assert.Empty(report.DomainEvents);
        Assert.Empty(report.DistributedEvents);
    }

    [Fact]
    public void IsEmpty_WhenEmpty_ReturnsTrue()
    {
        var report = new EntityChangeReport();
        Assert.True(report.IsEmpty());
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var report = new EntityChangeReport();
        var str = report.ToString();
        Assert.Contains("EntityChangeReport", str);
        Assert.Contains("ChangedEntities: 0", str);
        Assert.Contains("DomainEvents: 0", str);
        Assert.Contains("DistributedEvents: 0", str);
    }
}
