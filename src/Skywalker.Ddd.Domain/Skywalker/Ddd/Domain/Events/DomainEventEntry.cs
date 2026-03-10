namespace Skywalker.Ddd.Domain.Events;

[Serializable]
public class DomainEventEntry
{
    public object SourceEntity { get; }

    public object EventData { get; }

    public DomainEventEntry(object sourceEntity, object eventData)
    {
        SourceEntity = sourceEntity;
        EventData = eventData;
    }
}
