// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Skywalker;
using Skywalker.Extensions.DynamicProxies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker DDD 核心服务，自动发现并注册所有引用程序集中的服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 此方法会：
    /// <list type="bullet">
    /// <item>发现所有引用了 Skywalker 的程序集</item>
    /// <item>基于反射扫描并注册所有标记了 DI 接口的服务</item>
    /// <item>注册动态代理服务 (Castle.DynamicProxy)</item>
    /// </list>
    /// <para>
    /// 外部模块通过 Builder 扩展方法按需添加：
    /// <code>
    /// services.AddSkywalker()
    ///     .AddEntityFramework&lt;MyDbContext&gt;(options => options.UseSqlServer(...))
    ///     .AddRedisCaching()
    ///     .AddRabbitMQEventBus();
    /// </code>
    /// </para>
    /// </remarks>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services)
    {
        return services.AddSkywalker(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// 添加 Skywalker DDD 核心服务，从指定程序集开始发现并注册所有服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="entryAssembly">入口程序集。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services, Assembly entryAssembly)
    {
        // 1. 创建并配置 PartManager
        var partManager = new SkywalkerPartManager();
        partManager.DiscoverAssemblies(entryAssembly);

        // 2. 注册 PartManager 供后续使用
        services.AddSingleton(partManager);

        // 3. 基于反射扫描注册所有服务
        partManager.RegisterAllServices(services);

        // 4. 注册动态代理服务
        services.AddDynamicProxies();
        services.AddInterceptedServices(partManager.Assemblies.ToArray());

        return new SkywalkerBuilder(services, partManager);
    }
}
