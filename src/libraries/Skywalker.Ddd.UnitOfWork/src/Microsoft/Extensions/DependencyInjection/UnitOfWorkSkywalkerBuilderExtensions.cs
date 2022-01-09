using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class UnitOfWorkSkywalkerBuilderExtensions
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        //skywalkerBuilder.Services.AddScoped(typeof(IExecuteNonQueryPipelineBehavior<>), typeof(UnitOfWorkExecuteQueryPipelineBehavior<>));
        services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        services.TryAddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        return services;
    }
}
