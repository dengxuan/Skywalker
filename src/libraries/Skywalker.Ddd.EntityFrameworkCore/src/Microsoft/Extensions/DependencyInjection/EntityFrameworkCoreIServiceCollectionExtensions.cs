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

}
