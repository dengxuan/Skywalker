// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.DynamicProxies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// UnitOfWork 服务扩展方法。
/// </summary>
public static class UowServiceCollectionExtensions
{
    /// <summary>
    /// 添加 UnitOfWork 模块服务。
    /// </summary>
    /// <param name="builder">Skywalker 构建器。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddUnitOfWork(this ISkywalkerBuilder builder)
    {
        var services = builder.Services;

        // Singleton
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
        services.TryAddSingleton<IUnitOfWorkTransactionBehaviourProvider, NullUnitOfWorkTransactionBehaviourProvider>();

        // AmbientUnitOfWork — 共享实例，确保 IAmbientUnitOfWork 和 IUnitOfWorkAccessor 使用同一实例
        services.TryAddSingleton<AmbientUnitOfWork>();
        services.TryAddSingleton<IAmbientUnitOfWork>(sp => sp.GetRequiredService<AmbientUnitOfWork>());
        services.TryAddSingleton<IUnitOfWorkAccessor>(sp => sp.GetRequiredService<AmbientUnitOfWork>());

        // Transient
        services.TryAddTransient<IUnitOfWork, UnitOfWork>();
        services.TryAddTransient<IInterceptor, UnitOfWorkInterceptor>();

        return builder;
    }
}
