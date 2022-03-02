using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection.Abstractions;
using Skywalker.ObjectMapper;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Domain.Entities.Events.Distributed;

[TransientDependency]
public class EntityToEtoMapper : IEntityToEtoMapper
{
    protected IHybridServiceScopeFactory HybridServiceScopeFactory { get; }

    protected SkywalkerDistributedEntityEventOptions Options { get; }

    public EntityToEtoMapper(IOptions<SkywalkerDistributedEntityEventOptions> options, IHybridServiceScopeFactory hybridServiceScopeFactory)
    {
        HybridServiceScopeFactory = hybridServiceScopeFactory;
        Options = options.Value;
    }

    public object? Map(object @object)
    {
        Check.NotNull(@object, nameof(@object));

        if (@object is not IEntity entity)
        {
            return null;
        }

        var entityType = entity.GetType();
        var etoMappingItem = Options.EtoMappings.GetOrDefault(entityType);
        if (etoMappingItem == null)
        {
            var keys = entity.GetKeys().JoinAsString(",");
            return new EntityEto(entityType.FullName!, keys);
        }

        using var scope = HybridServiceScopeFactory.CreateScope();
        var objectMapperType = etoMappingItem.ObjectMappingContextType == null ? typeof(IObjectMapper) : typeof(IObjectMapper<>).MakeGenericType(etoMappingItem.ObjectMappingContextType);

        var objectMapper = (IObjectMapper)scope.ServiceProvider.GetRequiredService(objectMapperType);

        return objectMapper.Map(entityType, etoMappingItem.EtoType, @object);
    }
}
