using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities.Events.Distributed;
using Skywalker.Ddd.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DomainIServiceCollectionRepositoryExtensions
{
    public static partial IServiceCollection AddDomainServices(this IServiceCollection services);

    internal static IServiceCollection AddDomainServicesCore(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<SkywalkerDbConnectionOptions>(configuration);
        services.AddSingleton<IDataSeeder, DataSeeder>();
        services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
        services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
        services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        services.AddTransient(typeof(IDomainService<>),typeof(IDomainService<>));
        services.AddTransient(typeof(IDomainService<,>), typeof(IDomainService<,>));
        return services;
    }

    public static partial IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddDomainServicesCore();
        return services;
    }
}
