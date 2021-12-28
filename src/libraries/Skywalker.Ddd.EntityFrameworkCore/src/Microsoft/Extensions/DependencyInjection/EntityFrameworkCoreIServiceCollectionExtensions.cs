using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Extensions.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextBuilder> builderAction)
    {
        services.AddDomainServicesCore();
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
        services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
        var builder = new SkywalkerDbContextBuilder(services);
        builderAction(builder);
        return services;
    }
}
