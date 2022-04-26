using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore<TDbContext>(this IServiceCollection services, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        services.Configure(options);
        services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.TryAddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        services.AddDbContext<TDbContext>();
        return services;
    }


#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
    //public static SkywalkerDbContextOptions ConfigurePooledDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<DbContextOptionsBuilder> builder, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
    //{
    //    options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
    //    options.Services.AddPooledDbContextFactory<TDbContext>(builder, poolSize);
    //    return options;
    //}

    //public static SkywalkerDbContextOptions ConfigureDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<DbContextOptionsBuilder> builder) where TDbContext : SkywalkerDbContext<TDbContext>
    //{
    //    options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
    //    options.Services.AddDbContextFactory<TDbContext>(builder);
    //    return options;
    //}
#endif

}
