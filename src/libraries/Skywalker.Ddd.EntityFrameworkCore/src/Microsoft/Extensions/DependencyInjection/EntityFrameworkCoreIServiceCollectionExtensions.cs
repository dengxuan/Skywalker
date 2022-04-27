using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> options)
    {
        services.Configure(options);
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        services.TryAddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        return services;
    }

    public static SkywalkerDbContextOptions ConfigureDbContext<TDbContext>(this SkywalkerDbContextOptions options, Action<DbContextOptionsBuilder> builder) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContext<TDbContext>(builder);
        return options;
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
    public static SkywalkerDbContextOptions ConfigureDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<DbContextOptionsBuilder> builder) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContextFactory<TDbContext>(builder);
        return options;
    }

    public static SkywalkerDbContextOptions ConfigurePooledDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<DbContextOptionsBuilder> builder, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddPooledDbContextFactory<TDbContext>(builder, poolSize);
        return options;
    }
#endif

}
