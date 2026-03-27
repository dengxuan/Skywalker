// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker 核心服务（UnitOfWork），返回构建器用于链式注册其他模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>Skywalker 构建器。</returns>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services)
    {
        services.AddUnitOfWork();
        return new SkywalkerBuilder(services);
    }
}
