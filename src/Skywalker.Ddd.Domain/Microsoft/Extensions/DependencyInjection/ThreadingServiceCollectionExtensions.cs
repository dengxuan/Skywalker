// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.Threading;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Threading 服务注册扩展方法。
/// </summary>
public static class ThreadingServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Threading 基础服务（CancellationToken、AmbientScope）。
    /// </summary>
    public static IServiceCollection AddThreading(this IServiceCollection services)
    {
        services.TryAddScoped<ICancellationTokenProvider>(sp => NullCancellationTokenProvider.Instance);
        services.TryAddSingleton(typeof(IAmbientScopeProvider<>), typeof(AmbientDataContextAmbientScopeProvider<>));
        return services;
    }
}
