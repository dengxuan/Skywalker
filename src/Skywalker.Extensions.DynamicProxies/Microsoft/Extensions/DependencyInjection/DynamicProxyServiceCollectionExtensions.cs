// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.DynamicProxies;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 动态代理的依赖注入扩展方法。
/// </summary>
public static class DynamicProxyServiceCollectionExtensions
{
    /// <summary>
    /// 添加动态代理服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddDynamicProxies(this IServiceCollection services)
    {
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
        var generatedProxies = GetGeneratedDynamicProxies();

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

            if (!generatedProxies.TryGetValue((serviceType, implType), out var proxyType))
            {
                if (!HasInterceptableMethods(serviceType))
                {
                    continue;
                }

                throw new InvalidOperationException(
                    $"Service '{serviceType.FullName}' implemented by '{implType.FullName}' is marked with IInterceptable, but no source-generated DynamicProxy metadata was found. " +
                    "Reference Skywalker.Extensions.DynamicProxies.SourceGenerators as an analyzer and use a supported interface proxy shape, or remove IInterceptable from this registration. Castle.DynamicProxy fallback was removed in Skywalker v2.0.");
            }

            var proxyDescriptor = new ServiceDescriptor(serviceType, sp => CreateGeneratedProxy(sp, implType, proxyType), lifetime);

            services.Replace(proxyDescriptor);
        }

        return services;
    }

    private static Dictionary<(Type ServiceType, Type ImplementationType), Type> GetGeneratedDynamicProxies()
    {
        var proxies = new Dictionary<(Type ServiceType, Type ImplementationType), Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var attribute in assembly.GetCustomAttributes(typeof(SkywalkerGeneratedDynamicProxyAttribute)).OfType<SkywalkerGeneratedDynamicProxyAttribute>())
            {
                proxies[(attribute.ServiceType, attribute.ImplementationType)] = attribute.ProxyType;
            }
        }

        return proxies;
    }

    private static bool HasInterceptableMethods(Type serviceType)
    {
        return serviceType.GetMethods().Any(static method => !method.IsSpecialName);
    }

    private static object CreateGeneratedProxy(IServiceProvider serviceProvider, Type implementationType, Type proxyType)
    {
        var target = serviceProvider.GetRequiredService(implementationType);
        var interceptors = serviceProvider.GetServices<IInterceptor>();
        var constructor = proxyType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: new[] { implementationType, typeof(IEnumerable<IInterceptor>) },
            modifiers: null);
        if (constructor is null)
        {
            throw new InvalidOperationException($"Generated proxy type '{proxyType.FullName}' does not expose the expected constructor.");
        }

        return constructor.Invoke(new object[] { target, interceptors });
    }
}
