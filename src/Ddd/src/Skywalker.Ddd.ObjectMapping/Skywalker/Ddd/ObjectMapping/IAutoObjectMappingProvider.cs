using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.ObjectMapping
{
    public interface IAutoObjectMappingProvider : ISingletonDependency
    {
        TDestination Map<TSource, TDestination>(object source);

        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
    }

    public interface IAutoObjectMappingProvider<TContext> : IAutoObjectMappingProvider
    {

    }
}