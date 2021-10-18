using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.Uow.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class UnitOfWorkSkywalkerBuilderExtensions
{
    public static SkywalkerBuilder AddUnitOfWork(this SkywalkerBuilder skywalkerBuilder)
    {
        //skywalkerBuilder.Services.AddScoped(typeof(IExecuteNonQueryPipelineBehavior<>), typeof(UnitOfWorkExecuteQueryPipelineBehavior<>));
        skywalkerBuilder.Services.TryAddTransient(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
        return skywalkerBuilder;
    }
}
