using Skywalker.Data.Filtering;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding data filtering services.
/// </summary>
public static class DataFilteringServiceCollectionExtensions
{
    /// <summary>
    /// Adds data filtering services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDataFiltering(this IServiceCollection services)
    {
        return services.AddDataFiltering(_ => { });
    }

    /// <summary>
    /// Adds data filtering services with configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDataFiltering(
        this IServiceCollection services,
        Action<DataFilterOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IDataFilter, DataFilter>();
        services.AddTransient(typeof(IDataFilter<>), typeof(DataFilter<>));

        // Configure default filters
        services.Configure<DataFilterOptions>(options =>
        {
            // Soft delete filter is enabled by default
            if (!options.DefaultStates.ContainsKey(typeof(ISoftDelete)))
            {
                options.Configure<ISoftDelete>(true);
            }

            // Multi-tenant filter is enabled by default
            if (!options.DefaultStates.ContainsKey(typeof(IMultiTenant)))
            {
                options.Configure<IMultiTenant>(true);
            }
        });

        return services;
    }
}

