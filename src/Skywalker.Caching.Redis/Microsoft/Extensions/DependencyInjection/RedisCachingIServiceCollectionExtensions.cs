using Skywalker.Caching.Redis;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Redis caching services.
/// </summary>
public static class RedisCachingIServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis caching services with configuration from appsettings.json.
    /// Configuration section: Skywalker:Caching:Redis
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRedisCaching(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<RedisOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        SkywalkerCachingRedisAutoServiceExtensions.AddAutoServices(services);
        return services;
    }

    /// <summary>
    /// Adds Redis caching services with configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisOptions> configure)
    {
        var configuration = services.GetConfiguration();
        if (configuration != null)
        {
            services.AddOptions<RedisOptions>()
                .Bind(configuration.GetSection(RedisOptions.SectionName))
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        else
        {
            services.AddOptions<RedisOptions>()
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        SkywalkerCachingRedisAutoServiceExtensions.AddAutoServices(services);
        return services;
    }
}
