using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static DomainBuilder AddEntityFrameworkCore(this DomainBuilder domainBuilder)
    {
        domainBuilder.Services.TryAddTransient<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
        domainBuilder.Services.TryAddSingleton<IConnectionStringResolver, DefaultConnectionStringResolver>();
        domainBuilder.Services.TryAddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        return domainBuilder;
    }

    public static DomainBuilder AddDbContext<TDbContext>(this DomainBuilder domainBuilder, Action<SkywalkerDbContextOptions> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        domainBuilder.Services.Configure(options);
        domainBuilder.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        domainBuilder.Services.AddDbContext<TDbContext>();
        return domainBuilder;
    }

    public static DomainBuilder AddDbContextPool<TDbContext>(this DomainBuilder domainBuilder, Action<DbContextOptionsBuilder> options) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        domainBuilder.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        domainBuilder.Services.AddSingleton(typeof(IDbContextProvider<TDbContext>), typeof(UnitOfWorkDbContextProvider<TDbContext>));
        domainBuilder.Services.AddDbContextPool<TDbContext>(options);
        return domainBuilder;
    }

    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> optionsAction)
    {
        var options = new SkywalkerDbContextOptions(services);
        services.AddSingleton(options);
        services.AddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.AddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        services.AddSingleton<IConnectionStringResolver, DefaultConnectionStringResolver>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        optionsAction(options);
        return services;
    }

}
