// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker;
using Skywalker.ApplicationParts;
using Skywalker.Extensions.DynamicProxies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker DDD 核心服务，发现所有引用程序集并自动注册各模块服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    /// <remarks>
    /// 此方法会：
    /// <list type="number">
    /// <item>发现所有引用了 Skywalker 的程序集，包装为 <see cref="AssemblyPart"/></item>
    /// <item>从程序集中自动发现 <see cref="IApplicationFeatureProvider{ServiceRegistrationFeature}"/> 实现</item>
    /// <item>通过 FeatureProvider 机制收集并注册所有服务</item>
    /// </list>
    /// 各模块引用即生效，无需手动链式调用。如需替换默认实现（如将本地 EventBus 替换为 RabbitMQ），
    /// 在 <c>AddSkywalker()</c> 之后调用对应的替换方法即可（因为使用 <c>TryAdd</c> 语义，先注册的优先）。
    /// <code>
    /// services.AddSkywalker();                    // 自动注册所有引用的模块
    /// services.AddEventBusRabbitMQ(options => {}); // 替换默认的本地 EventBus
    /// </code>
    /// </remarks>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services)
    {
        return services.AddSkywalker(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// 添加 Skywalker DDD 核心服务，从指定程序集开始发现并自动注册各模块服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="entryAssembly">入口程序集。</param>
    /// <returns>Skywalker 构建器，支持链式调用。</returns>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services, Assembly entryAssembly)
    {
        var partManager = new SkywalkerPartManager();
        partManager.PopulateDefaultParts(entryAssembly);

        services.TryAddSingleton(partManager);

        // 从发现的 Part 中扫描 IApplicationFeatureProvider<ServiceRegistrationFeature> 实现
        var featureProviderType = typeof(IApplicationFeatureProvider<ServiceRegistrationFeature>);
        foreach (var part in partManager.ApplicationParts.OfType<AssemblyPart>())
        {
            foreach (var type in part.Types)
            {
                if (type.IsClass && !type.IsAbstract && featureProviderType.IsAssignableFrom(type))
                {
                    var provider = (IApplicationFeatureProvider<ServiceRegistrationFeature>)Activator.CreateInstance(type)!;
                    partManager.FeatureProviders.Add(provider);
                }
            }
        }

        // 通过 PartManager 统一收集所有服务描述符
        var feature = new ServiceRegistrationFeature { ServiceCollection = services };
        partManager.PopulateFeature(feature);

        // 注册所有发现的服务（TryAdd 语义，先注册的优先）
        foreach (var descriptor in feature.Services)
        {
            services.TryAdd(descriptor);
        }

        // 应用替换注册（[ReplaceService] 标记的服务）
        foreach (var descriptor in feature.Replacements)
        {
            services.Replace(descriptor);
        }

        // 为实现了 IInterceptable 的服务启用动态代理
        services.AddInterceptedServices();

        return new SkywalkerBuilder(services, partManager);
    }
}
