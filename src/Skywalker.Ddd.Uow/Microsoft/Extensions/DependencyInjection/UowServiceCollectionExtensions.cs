// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// UnitOfWork 服务扩展方法。
/// </summary>
public static class UowServiceCollectionExtensions
{
    /// <summary>
    /// 单独添加 UnitOfWork 模块服务到服务集合。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    /// <remarks>
    /// 通常不需要直接调用此方法，<see cref="SkywalkerServiceCollectionExtensions.AddSkywalker"/>
    /// 会自动注册所有 Skywalker 模块的服务。
    /// </remarks>
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        return services;
    }
}
