// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Skywalker;
using Skywalker.ApplicationParts;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker DDD 核心服务，发现所有引用程序集并创建 <see cref="SkywalkerPartManager"/>。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 此方法会发现所有引用了 Skywalker 的程序集，包装为 <see cref="AssemblyPart"/>。
    /// 各模块通过 Builder 扩展方法按需添加 FeatureProvider 并注册服务：
    /// <code>
    /// services.AddSkywalker()
    ///     .AddUnitOfWork()
    ///     .AddDddDomain()
    ///     .AddDddApplication()
    ///     .AddEntityFramework&lt;MyDbContext&gt;(options => options.UseSqlServer(...));
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services)
    {
        return services.AddSkywalker(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// 添加 Skywalker DDD 核心服务，从指定程序集开始发现并创建 <see cref="SkywalkerPartManager"/>。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="entryAssembly">入口程序集。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services, Assembly entryAssembly)
    {
        var partManager = new SkywalkerPartManager();
        partManager.PopulateDefaultParts(entryAssembly);

        services.AddSingleton(partManager);

        return new SkywalkerBuilder(services, partManager);
    }
}
