using Microsoft.EntityFrameworkCore;
using Skywalker.Localization.EntityFrameworkCore;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding EF Core localization services to the DI container.
/// </summary>
public static class LocalizationEfCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds EF Core localization services to the service collection.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type that implements ILocalizationDbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLocalizationEfCore<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, ILocalizationDbContext
    {
        services.AddScoped<ILocalizationDbContext>(sp => sp.GetRequiredService<TDbContext>());
        // LocalizationTextManager 通过 IScopedDependency 自动注册
        SkywalkerLocalizationEntityFrameworkCoreAutoServiceExtensions.AddAutoServices(services);
        services.AddMemoryCache();

        return services;
    }

    /// <summary>
    /// Adds EF Core localization services with a custom DbContext configuration.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type that implements ILocalizationDbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLocalizationEfCore<TDbContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : DbContext, ILocalizationDbContext
    {
        services.AddDbContext<TDbContext>(optionsAction);
        return services.AddLocalizationEfCore<TDbContext>();
    }
}

