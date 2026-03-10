using Skywalker.ObjectMapping;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding object mapping services.
/// </summary>
public static class ObjectMappingServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default object mapper.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddObjectMapping(this IServiceCollection services)
    {
        services.AddSingleton<IObjectMapper, DefaultObjectMapper>();
        return services;
    }
}

