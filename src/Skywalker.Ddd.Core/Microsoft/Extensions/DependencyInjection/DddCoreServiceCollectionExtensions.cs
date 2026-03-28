// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd;
using Skywalker.Ddd.Abstractions;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// DDD Core 服务扩展方法。
/// </summary>
public static class DddCoreServiceCollectionExtensions
{
    /// <summary>
    /// 添加 DDD Core 服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddDddCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddAutoServices();
        services.AddEventBusCore();

        return services;
    }
}
