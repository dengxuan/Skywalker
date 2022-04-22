using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

public interface IEntityToEtoMapper: ITransientDependency
{
    object? Map(object entityObj);
}
