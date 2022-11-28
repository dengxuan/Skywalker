using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static DddBuilder AddEntityFrameworkCore(this DddBuilder builder)
    {
        builder.AddUnitOfWork();
        builder.Services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
        builder.Services.TryAddSingleton<IConnectionStringResolver, DefaultConnectionStringResolver>();
        builder.Services.TryAddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        return builder;
    }

    public static DddBuilder AddDbContext<TDbContext>(this DddBuilder builder, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        builder.Services.Configure(options);
        builder.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        builder.Services.AddDbContext<TDbContext>();
        return builder;
    }

    //public static DddBuilder AddDbContextPool<TDbContext>(this DddBuilder builder, Action<DbContextOptionsBuilder> options) where TDbContext : SkywalkerDbContext<TDbContext>
    //{
    //    builder.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
    //    builder.Services.AddSingleton(typeof(IDbContextProvider<TDbContext>), typeof(UnitOfWorkDbContextProvider<TDbContext>));
    //    builder.Services.AddDbContextPool<TDbContext>(options);
    //    return builder;
    //}

    //public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> optionsAction)
    //{
    //    var options = new SkywalkerDbContextOptions(services);
    //    services.AddSingleton(options);
    //    services.AddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
    //    services.AddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
    //    services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
    //    services.AddSingleton<IConnectionStringResolver, DefaultConnectionStringResolver>();
    //    services.AddScoped<IUnitOfWork, UnitOfWork>();
    //    optionsAction(options);
    //    return services;
    //}

}
