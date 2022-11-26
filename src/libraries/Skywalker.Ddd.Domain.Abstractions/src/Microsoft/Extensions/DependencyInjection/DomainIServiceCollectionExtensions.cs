// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class DomainIServiceCollectionExtensions
{
    internal static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        services.TryAddTransient<IDatabaseApiContainer, UnitOfWork>();
        services.TryAddTransient<ITransactionApiContainer, UnitOfWork>();

        var ambientUnitOfWork = new AmbientUnitOfWork();
        services.TryAddSingleton<IAmbientUnitOfWork>(ambientUnitOfWork);
        services.TryAddSingleton<IUnitOfWorkAccessor>(ambientUnitOfWork);

        services.TryAddSingleton<IUnitOfWorkManager,UnitOfWorkManager>();

        return services;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static DomainBuilder AddDddCore(this IServiceCollection services)
    {
        var domainBuilder = new DomainBuilder(services);
        services.AddObjectAccessor(domainBuilder);
        services.TryAddSingleton<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
        services.AddUnitOfWork();
        return domainBuilder;
    }
}
