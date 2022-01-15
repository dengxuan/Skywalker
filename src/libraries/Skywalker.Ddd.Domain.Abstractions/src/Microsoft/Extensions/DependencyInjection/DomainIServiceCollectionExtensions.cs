using Skywalker;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Data.Seeding;
using Skywalker.Ddd.Domain.Entities.Events.Distributed;
using Skywalker.Ddd.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DomainIServiceCollectionExtensions
{
    private static readonly HashSet<Action<IServiceCollection>> s_configureServices = new();

    public static void ConfigureServices(Action<IServiceCollection> action)
    {
        Check.NotNull(action, nameof(action));
        s_configureServices.Add(action);
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<SkywalkerDbConnectionOptions>(configuration);
        services.AddSingleton<IDataSeeder, DataSeeder>();
        services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();
        services.AddTransient<IEntityToEtoMapper, EntityToEtoMapper>();
        services.AddSingleton(typeof(IDataFilter<>), typeof(DataFilter<>));
        services.AddTransient(typeof(IDomainService<>), typeof(DomainService<>));
        services.AddTransient(typeof(IDomainService<,>), typeof(DomainService<,>));
        s_configureServices.ForEach(action => action(services));
        return services;
    }
}
