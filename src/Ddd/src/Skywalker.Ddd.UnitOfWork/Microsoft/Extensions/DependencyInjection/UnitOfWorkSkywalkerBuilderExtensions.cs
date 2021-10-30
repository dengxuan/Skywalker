using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class UnitOfWorkSkywalkerBuilderExtensions
{
    public static SkywalkerDddBuilder AddUnitOfWork(this SkywalkerDddBuilder skywalkerBuilder)
    {
        //skywalkerBuilder.Services.AddScoped(typeof(IExecuteNonQueryPipelineBehavior<>), typeof(UnitOfWorkExecuteQueryPipelineBehavior<>));
        skywalkerBuilder.Services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        skywalkerBuilder.Services.TryAddSingleton<IAmbientUnitOfWork, AmbientUnitOfWork>();
        skywalkerBuilder.Services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        skywalkerBuilder.Services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        return skywalkerBuilder;
    }
}
