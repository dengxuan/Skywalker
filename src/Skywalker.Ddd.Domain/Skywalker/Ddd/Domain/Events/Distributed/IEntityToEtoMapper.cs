namespace Skywalker.Ddd.Domain.Events.Distributed;

public interface IEntityToEtoMapper
{
    object? Map(object entityObj);
}
