// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.DynamicProxies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 动态代理的依赖注入扩展方法。
/// </summary>
public static class DynamicProxyServiceCollectionExtensions
{
    /// <summary>
    /// 添加动态代理服务（Castle.DynamicProxy）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddDynamicProxies(this IServiceCollection services)
    {
        services.TryAddSingleton<IProxyGenerator, CastleProxyGenerator>();
        return services;
    }

    /// <summary>
    /// 扫描已注册的服务，为实现了 <see cref="IInterceptable"/> 的服务启用代理。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    /// <remarks>
    /// 此方法会遍历已注册的 <see cref="ServiceDescriptor"/>，
    /// 找到实现类型实现了 <see cref="IInterceptable"/> 的服务，
    /// 用代理工厂替换原有注册，保持原有生命周期不变。
    /// 无需任何 DI 标记接口，只要服务已注册且实现了 <see cref="IInterceptable"/> 即可。
    /// </remarks>
    public static IServiceCollection AddInterceptedServices(this IServiceCollection services)
    {
        services.AddDynamicProxies();

        // 快照当前注册，避免在遍历时修改集合
        var descriptors = services.ToList();

        foreach (var descriptor in descriptors)
        {
            var implType = descriptor.ImplementationType;
            if (implType == null || !typeof(IInterceptable).IsAssignableFrom(implType))
            {
                continue;
            }

            // 跳过注册为自身的描述符（保留它，代理工厂需要从它解析原始实例）
            if (descriptor.ServiceType == implType)
            {
                continue;
            }

            // 跳过接口类型的服务类型（代理只能包装具体接口）
            if (!descriptor.ServiceType.IsInterface)
            {
                continue;
            }

            var serviceType = descriptor.ServiceType;
            var lifetime = descriptor.Lifetime;

            // 确保实现类型本身已注册（代理工厂需要解析原始实例）
            services.TryAdd(new ServiceDescriptor(implType, implType, lifetime));

            // 用代理工厂替换原有注册
            var proxyDescriptor = new ServiceDescriptor(
                serviceType,
                sp =>
                {
                    var target = sp.GetRequiredService(implType);
                    var proxyGenerator = sp.GetRequiredService<IProxyGenerator>();
                    return proxyGenerator.CreateInterfaceProxy(serviceType, target);
                },
                lifetime);

            services.Replace(proxyDescriptor);
        }

        return services;
    }
}
