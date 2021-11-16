using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.DependencyInjection;
using Skywalker.Ddd.ObjectMapping;
using System.Collections.Generic;
using Skywalker.Ddd.DependencyInjection;

namespace Skywalker.Domain.Entities.Events.Distributed
{
    public class EntityToEtoMapper : IEntityToEtoMapper/*, ITransientDependency*/
    {
        protected IHybridServiceScopeFactory HybridServiceScopeFactory { get; }
        
        protected SkywalkerDistributedEntityEventOptions Options { get; }

        public EntityToEtoMapper(
            IOptions<SkywalkerDistributedEntityEventOptions> options,
            IHybridServiceScopeFactory hybridServiceScopeFactory)
        {
            HybridServiceScopeFactory = hybridServiceScopeFactory;
            Options = options.Value;
        }

        public object Map(object entityObj)
        {
            Check.NotNull(entityObj, nameof(entityObj));

            if (!(entityObj is IEntity entity))
            {
                return null;
            }

            var entityType = entity.GetType();
            var etoMappingItem = Options.EtoMappings.GetOrDefault(entityType);
            if (etoMappingItem == null)
            {
                var keys = entity.GetKeys().JoinAsString(",");
                return new EntityEto(entityType.FullName, keys);
            }

            using var scope = HybridServiceScopeFactory.CreateScope();
            var objectMapperType = etoMappingItem.ObjectMappingContextType == null
                ? typeof(IObjectMapper)
                : typeof(IObjectMapper<>).MakeGenericType(etoMappingItem.ObjectMappingContextType);

            var objectMapper = (IObjectMapper)scope.ServiceProvider.GetRequiredService(objectMapperType);

            return objectMapper.Map(entityType, etoMappingItem.EtoType, entityObj);
        }
    }
}