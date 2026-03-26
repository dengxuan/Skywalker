// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker DDD 整合包服务扩展。
/// </summary>
public static class DddServiceCollectionExtensions
{
    /// <summary>
    /// 添加 DDD 完整功能（包含 Core + EntityFrameworkCore）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddDdd(this IServiceCollection services)
    {
        services.AddDddCore();
        services.AddEntityFrameworkCore();
        return services;
    }
}
