// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker DDD 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker DDD 全部核心服务：UnitOfWork、异常处理、响应包装。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddSkywalker(this IServiceCollection services)
    {
        services.AddUnitOfWork();
        services.AddSkywalkerExceptionHandling();
        services.AddResponseWrapper();
        return services;
    }
}
