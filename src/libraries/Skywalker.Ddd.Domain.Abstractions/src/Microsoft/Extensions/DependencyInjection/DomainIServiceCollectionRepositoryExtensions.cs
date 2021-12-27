using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities.Events.Distributed;

namespace Microsoft.Extensions.DependencyInjection;

public static class DomainIServiceCollectionRepositoryExtensions
{

    internal static IServiceCollection AddDomainServicesCore(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<SkywalkerDbConnectionOptions>(configuration);
        services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        services.AddSingleton<IDataSeeder, DataSeeder>();
        services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
        services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
        return services;
    }
}
