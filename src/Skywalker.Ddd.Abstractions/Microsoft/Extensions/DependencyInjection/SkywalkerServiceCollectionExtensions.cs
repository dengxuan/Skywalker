// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.ApplicationParts;

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
    /// <item>发现所有引用了 Skywalker 的程序集，包装为 <see cref="AssemblyPart"/></item>
    /// <item>通过 <see cref="ServiceRegistrationFeatureProvider"/> 扫描并注册所有标记了 DI 接口的服务</item>
    /// <item>为实现了 IInterceptable 的服务启用动态代理</item>
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
        // 1. 创建 PartManager 并发现所有 Skywalker 程序集
        var partManager = new SkywalkerPartManager();
        partManager.PopulateDefaultParts(entryAssembly);

        // 2. 添加默认的 FeatureProvider
        partManager.FeatureProviders.Add(new ServiceRegistrationFeatureProvider());

        // 3. 注册 PartManager 供后续使用
        services.AddSingleton(partManager);

        // 4. 通过 FeatureProvider 发现并注册服务
        var feature = new ServiceRegistrationFeature();
        partManager.PopulateFeature(feature);

        foreach (var descriptor in feature.Services)
        {
            var replaceAttr = descriptor.ImplementationType?.GetCustomAttribute<ReplaceServiceAttribute>();
            if (replaceAttr != null)
            {
                services.Replace(descriptor);
            }
            else
            {
                services.TryAdd(descriptor);
            }
        }

        // 5. 扫描已注册服务，为 IInterceptable 的服务启用代理
        services.AddInterceptedServices();

        return new SkywalkerBuilder(services, partManager);
    }
}
