using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class EntityFrameworkCoreIServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCore(this IServiceCollection services, Action<SkywalkerDbContextOptions> optionsAction)
    {
        var options = new SkywalkerDbContextOptions(services);
        services.AddSingleton(options);
        services.AddSingleton(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.AddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        services.AddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        optionsAction(options);
        return services;
    }

}
