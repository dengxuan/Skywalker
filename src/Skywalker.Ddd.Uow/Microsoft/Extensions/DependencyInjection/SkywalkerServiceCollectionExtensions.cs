// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Skywalker;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Skywalker 一站式服务注册扩展方法。
/// </summary>
public static class SkywalkerServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Skywalker 核心服务，自动发现并注册所有引用程序集中的服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>Skywalker 构建器。</returns>
    /// <remarks>
    /// 此方法会：
    /// <list type="bullet">
    /// <item>发现所有引用了 Skywalker 的程序集</item>
    /// <item>调用每个程序集中 SourceGenerator 生成的服务注册方法</item>
    /// <item>注册 UnitOfWork、拦截器等核心基础设施</item>
    /// </list>
    /// </remarks>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services)
    {
        return services.AddSkywalker(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// 添加 Skywalker 核心服务，从指定程序集开始发现并注册所有服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="entryAssembly">入口程序集。</param>
    /// <returns>Skywalker 构建器。</returns>
    public static ISkywalkerBuilder AddSkywalker(this IServiceCollection services, Assembly entryAssembly)
    {
        // 1. 创建并配置 PartManager
        var partManager = new SkywalkerPartManager();
        partManager.DiscoverAssemblies(entryAssembly);

        // 2. 注册 PartManager 供后续使用
        services.AddSingleton(partManager);

        // 3. 调用所有发现的程序集的服务注册方法
        partManager.RegisterAllServices(services);

        return new SkywalkerBuilder(services);
    }
}
