// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.Threading;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class DomainDddBuilderExtensions
{
    internal static DddBuilder AddUnitOfWork(this DddBuilder builder)
    {
        builder.Services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.TryAddTransient<IDatabaseApiContainer, UnitOfWork>();
        builder.Services.TryAddTransient<ITransactionApiContainer, UnitOfWork>();

        builder.Services.TryAddScoped<ICancellationTokenProvider,NullCancellationTokenProvider>();

        var ambientUnitOfWork = new AmbientUnitOfWork();
        builder.Services.TryAddSingleton<IAmbientUnitOfWork>(ambientUnitOfWork);
        builder.Services.TryAddSingleton<IUnitOfWorkAccessor>(ambientUnitOfWork);
        builder.Services.TryAddSingleton<IUnitOfWorkManager,UnitOfWorkManager>();

        builder.Services.TryAddSingleton<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
        builder.Services.TryAddSingleton(typeof(IAmbientScopeProvider<>),typeof(AmbientDataContextAmbientScopeProvider<>));
        return builder;
    }
}
