using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.DependencyInjection;

namespace Skywalker.Extensions.DynamicProxies;

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
        services.TryAddSingleton<IProxyGenerator, CastleProxyGenerator>();
        return services;
    }

    /// <summary>
    /// 为指定程序集中实现 <see cref="IInterceptable"/> 的服务启用代理。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assemblies">要扫描的程序集。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddInterceptedServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddDynamicProxies();

        foreach (var assembly in assemblies)
        {
            AddInterceptedServicesFromAssembly(services, assembly);
        }

        return services;
    }

    private static void AddInterceptedServicesFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces().Any(i => i == typeof(IInterceptable)));

        foreach (var implementationType in types)
        {
            var lifetime = GetServiceLifetime(implementationType);
            if (lifetime == null)
            {
                continue;
            }

            // 找到 IInterceptable 接口以外的服务接口
            var serviceInterfaces = implementationType.GetInterfaces()
                .Where(i => i != typeof(IInterceptable))
                .Where(i => !IsMarkerInterface(i))
                .ToList();

            foreach (var serviceType in serviceInterfaces)
            {
                RegisterProxiedService(services, serviceType, implementationType, lifetime.Value);
            }
        }
    }

    private static ServiceLifetime? GetServiceLifetime(Type type)
    {
        if (typeof(ISingletonDependency).IsAssignableFrom(type))
        {
            return ServiceLifetime.Singleton;
        }

        if (typeof(IScopedDependency).IsAssignableFrom(type))
        {
            return ServiceLifetime.Scoped;
        }

        if (typeof(ITransientDependency).IsAssignableFrom(type))
        {
            return ServiceLifetime.Transient;
        }

        return null;
    }

    private static bool IsMarkerInterface(Type type)
    {
        return type == typeof(ISingletonDependency) ||
               type == typeof(IScopedDependency) ||
               type == typeof(ITransientDependency) ||
               type == typeof(IInterceptable) ||
               type == typeof(IDisposable) ||
               type == typeof(IAsyncDisposable);
    }

    private static void RegisterProxiedService(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime)
    {
        // 先注册实现类型
        var implDescriptor = new ServiceDescriptor(implementationType, implementationType, lifetime);
        services.TryAdd(implDescriptor);

        // 然后注册代理工厂
        var proxyDescriptor = new ServiceDescriptor(
            serviceType,
            sp =>
            {
                var target = sp.GetRequiredService(implementationType);
                var proxyGenerator = sp.GetRequiredService<IProxyGenerator>();
                return proxyGenerator.CreateInterfaceProxy(serviceType, target);
            },
            lifetime);

        services.Replace(proxyDescriptor);
    }
}
