using Skywalker.Data;
using Skywalker.Data.Filtering;
using Skywalker.Data.Seeding;
using Skywalker.Domain.Entities.Events.Distributed;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionRepositoryExtensions
{

    internal static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<SkywalkerDbConnectionOptions>(configuration);

        services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        //services.AddSingleton<IDataSeeder, DataSeeder>();
        services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
        //services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
        return services;
    }
}
