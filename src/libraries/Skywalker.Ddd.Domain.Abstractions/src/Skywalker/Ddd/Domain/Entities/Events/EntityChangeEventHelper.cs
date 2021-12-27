using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Entities.Events.Distributed;
using Skywalker.EventBus;
using Skywalker.EventBus.Abstractions;

namespace Skywalker.Ddd.Domain.Entities.Events;

/// <summary>
/// Used to trigger entity change events.
/// </summary>
public class EntityChangeEventHelper : IEntityChangeEventHelper
{
    public ILogger<EntityChangeEventHelper> Logger { get; set; }
    public IEventBus EventBus { get; set; }
    protected IEntityToEtoMapper EntityToEtoMapper { get; }
    protected SkywalkerDistributedEntityEventOptions DistributedEntityEventOptions { get; }

    public EntityChangeEventHelper(
        IEntityToEtoMapper entityToEtoMapper,
        IOptions<SkywalkerDistributedEntityEventOptions> distributedEntityEventOptions)
    {
        EntityToEtoMapper = entityToEtoMapper;
        DistributedEntityEventOptions = distributedEntityEventOptions.Value;

        EventBus = NullEventBus.Instance;
        Logger = NullLogger<EntityChangeEventHelper>.Instance;
    }

    public async Task TriggerEventsAsync(EntityChangeReport changeReport)
    {
        await TriggerEventsInternalAsync(changeReport);

        if (changeReport.IsEmpty())
        {
            return;
        }
    }

    public virtual async Task TriggerEntityCreatingEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityCreatingEventData<>),
            entity,
            entity,
            true
        );
    }

    public virtual async Task TriggerEntityCreatedEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityCreatedEventData<>),
            entity,
            entity,
            true
        );
    }

    public virtual async Task TriggerEntityCreatedEventOnUowCompletedAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityCreatedEventData<>),
            entity,
            entity,
            false
        );

        if (ShouldPublishDistributedEventForEntity(entity))
        {
            var eto = EntityToEtoMapper.Map(entity);
            if (eto != null)
            {
                await TriggerEventWithEntity(
                    EventBus,
                    typeof(EntityCreatedEto<>),
                    eto,
                    entity,
                    false
                );
            }
        }
    }

    private bool ShouldPublishDistributedEventForEntity(object entity)
    {
        return DistributedEntityEventOptions
            .AutoEventSelectors
            .IsMatch(entity.GetType()
            );
    }

    public virtual async Task TriggerEntityUpdatingEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityUpdatingEventData<>),
            entity,
            entity,
            true
        );
    }

    public virtual async Task TriggerEntityUpdatedEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityUpdatedEventData<>),
            entity,
            entity,
            true
        );
    }

    public virtual async Task TriggerEntityUpdatedEventOnUowCompletedAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityUpdatedEventData<>),
            entity,
            entity,
            false
        );

        if (ShouldPublishDistributedEventForEntity(entity))
        {
            var eto = EntityToEtoMapper.Map(entity);
            if (eto != null)
            {
                await TriggerEventWithEntity(
                    EventBus,
                    typeof(EntityUpdatedEto<>),
                    eto,
                    entity,
                    false
                );
            }
        }
    }

    public virtual async Task TriggerEntityDeletingEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityDeletingEventData<>),
            entity,
            entity,
            true
        );
    }

    public virtual async Task TriggerEntityDeletedEventAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityDeletedEventData<>),
            entity,
            entity,
            true
        );
    }
    public virtual async Task TriggerEntityDeletedEventOnUowCompletedAsync(object entity)
    {
        await TriggerEventWithEntity(
            EventBus,
            typeof(EntityDeletedEventData<>),
            entity,
            entity,
            false
        );

        if (ShouldPublishDistributedEventForEntity(entity))
        {
            var eto = EntityToEtoMapper.Map(entity);
            if (eto != null)
            {
                await TriggerEventWithEntity(
                    EventBus,
                    typeof(EntityDeletedEto<>),
                    eto,
                    entity,
                    false
                );
            }
        }
    }

    protected virtual async Task TriggerEventsInternalAsync(EntityChangeReport changeReport)
    {
        await TriggerEntityChangeEvents(changeReport.ChangedEntities);
        await TriggerDistributedEvents(changeReport.DistributedEvents);
    }

    protected virtual async Task TriggerEntityChangeEvents(List<EntityChangeEntry> changedEntities)
    {
        foreach (var changedEntity in changedEntities)
        {
            switch (changedEntity.ChangeType)
            {
                case EntityChangeType.Created:
                    await TriggerEntityCreatingEventAsync(changedEntity.Entity);
                    await TriggerEntityCreatedEventOnUowCompletedAsync(changedEntity.Entity);
                    break;
                case EntityChangeType.Updated:
                    await TriggerEntityUpdatingEventAsync(changedEntity.Entity);
                    await TriggerEntityUpdatedEventOnUowCompletedAsync(changedEntity.Entity);
                    break;
                case EntityChangeType.Deleted:
                    await TriggerEntityDeletingEventAsync(changedEntity.Entity);
                    await TriggerEntityDeletedEventOnUowCompletedAsync(changedEntity.Entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown EntityChangeType: " + changedEntity.ChangeType);
            }
        }
    }

    protected virtual async Task TriggerDistributedEvents(List<DomainEventEntry> distributedEvents)
    {
        foreach (var distributedEvent in distributedEvents)
        {
            await EventBus.PublishAsync(distributedEvent.EventData.GetType(),
                distributedEvent.EventData);
        }
    }

    protected virtual async Task TriggerEventWithEntity(
        IEventBus eventPublisher,
        Type genericEventType,
        object entityOrEto,
        object originalEntity,
        bool triggerInCurrentUnitOfWork)
    {
        var entityType = entityOrEto.GetType();
        var eventType = genericEventType.MakeGenericType(entityType);

        if (triggerInCurrentUnitOfWork)
        {
            var eventArgs = Activator.CreateInstance(eventType, entityOrEto);
            await eventPublisher.PublishAsync(eventType, eventArgs!);
            return;
        }

        var eventList = GetEventList();
        var isFirstEvent = !eventList.Any();

        eventList.AddUniqueEvent(eventPublisher, eventType, entityOrEto, originalEntity);

        /* Register to OnCompleted if this is the first item.
         * Other items will already be in the list once the UOW completes.
         */
        if (isFirstEvent)
        {
            foreach (var eventEntry in eventList)
            {
                try
                {
                    var eventArgs = Activator.CreateInstance(eventEntry.EventType, eventEntry.EntityOrEto);

                    await eventPublisher.PublishAsync(eventEntry.EventType, eventArgs!);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Caught an exception while publishing the event '{eventType.FullName}' for the entity '{entityOrEto}'", eventType.FullName, entityOrEto);
                }
            }
        }
    }

    private EntityChangeEventList GetEventList()
    {
        return new EntityChangeEventList();
    }

    private class EntityChangeEventList : List<EntityChangeEventEntry>
    {
        public void AddUniqueEvent(IEventBus eventBus, Type eventType, object entityOrEto, object originalEntity)
        {
            var newEntry = new EntityChangeEventEntry(eventBus, eventType, entityOrEto, originalEntity);

            //Latest "same" event overrides the previous events.
            for (var i = 0; i < Count; i++)
            {
                if (this[i].IsSameEvent(newEntry))
                {
                    this[i] = newEntry;
                    return;
                }
            }

            //If this is a "new" event, add to the end
            Add(newEntry);
        }
    }

    private class EntityChangeEventEntry
    {
        public IEventBus EventBus { get; }

        public Type EventType { get; }

        public object EntityOrEto { get; }

        public object OriginalEntity { get; }

        public EntityChangeEventEntry(IEventBus eventBus, Type eventType, object entityOrEto, object originalEntity)
        {
            EventType = eventType;
            EntityOrEto = entityOrEto;
            OriginalEntity = originalEntity;
            EventBus = eventBus;
        }

        public bool IsSameEvent(EntityChangeEventEntry otherEntry)
        {
            if (EventBus != otherEntry.EventBus || EventType != otherEntry.EventType)
            {
                return false;
            }

            if (OriginalEntity is not IEntity originalEntityRef || otherEntry.OriginalEntity is not IEntity otherOriginalEntityRef)
            {
                return false;
            }

            return EntityHelper.EntityEquals(originalEntityRef, otherOriginalEntityRef);
        }
    }
}
