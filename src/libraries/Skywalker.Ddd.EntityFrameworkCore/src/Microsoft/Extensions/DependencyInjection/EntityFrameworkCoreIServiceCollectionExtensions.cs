using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Extensions.Linq;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> options)
    {
        services.Configure(options);
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(DbContextProvider<>));
        services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
        return services;
    }

    public static IServiceCollection AddDbContextFactory<TDbContext>(this IServiceCollection services) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
#if NETSTANDARD2_0
        services.AddDbContext<TDbContext>();
#else
        services.AddDbContextFactory<TDbContext>();
#endif
        return services;
    }

    public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection services) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
#if NETSTANDARD2_0
        services.AddDbContext<TDbContext>();
#else
        services.AddDbContextFactory<TDbContext>();
#endif
        return services;
    }

}
