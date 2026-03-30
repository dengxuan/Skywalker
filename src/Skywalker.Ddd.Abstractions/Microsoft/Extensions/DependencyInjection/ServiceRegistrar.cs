using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 基于反射的服务自动注册器。
/// </summary>
/// <remarks>
/// 扫描程序集中实现依赖注入标记接口的类型并自动注册到 DI 容器。
/// </remarks>
public static class ServiceRegistrar
{
    /// <summary>
    /// 从指定程序集中扫描并注册所有标记了依赖注入接口的服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assemblies">要扫描的程序集。</param>
    public static void RegisterAssemblies(IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterAssembly(services, assembly);
        }
    }

    /// <summary>
    /// 从指定程序集中扫描并注册所有标记了依赖注入接口的服务。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="assembly">要扫描的程序集。</param>
    public static void RegisterAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => !t.IsDefined(typeof(DisableAutoRegistrationAttribute), false));

        foreach (var implementationType in types)
        {
            var lifetime = GetServiceLifetime(implementationType);
            if (lifetime == null)
            {
                continue;
            }

            var serviceTypes = GetServiceTypes(implementationType);
            foreach (var serviceType in serviceTypes)
            {
                RegisterService(services, serviceType, implementationType, lifetime.Value);
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

    private static IEnumerable<Type> GetServiceTypes(Type implementationType)
    {
        var exposeAttr = implementationType.GetCustomAttribute<ExposeServicesAttribute>();
        if (exposeAttr != null)
        {
            foreach (var serviceType in exposeAttr.ServiceTypes)
            {
                yield return serviceType;
            }

            if (exposeAttr.IncludeSelf)
            {
                yield return implementationType;
            }

            if (!exposeAttr.IncludeDefaults)
            {
                yield break;
            }
        }

        // 自动发现实现的接口（排除标记接口）
        var interfaces = implementationType.GetInterfaces()
            .Where(i => !IsMarkerInterface(i))
            .Where(i => !i.IsGenericType || !IsGenericMarkerInterface(i.GetGenericTypeDefinition()));

        foreach (var iface in interfaces)
        {
            yield return iface;
        }

        // 如果没有 ExposeServices 属性，也注册自身
        if (exposeAttr == null)
        {
            yield return implementationType;
        }
    }

    private static bool IsMarkerInterface(Type type)
    {
        return type == typeof(ISingletonDependency) ||
               type == typeof(IScopedDependency) ||
               type == typeof(ITransientDependency) ||
               type == typeof(IDisposable) ||
               type == typeof(IAsyncDisposable);
    }

    private static bool IsGenericMarkerInterface(Type type)
    {
        return false; // 可扩展以支持泛型标记接口
    }

    private static void RegisterService(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime)
    {
        var replaceAttr = implementationType.GetCustomAttribute<ReplaceServiceAttribute>();
        var keyedAttr = implementationType.GetCustomAttribute<KeyedServiceAttribute>();

        if (keyedAttr != null)
        {
            // 键控服务注册
            var descriptor = new ServiceDescriptor(serviceType, keyedAttr.Key, implementationType, lifetime);
            if (replaceAttr != null)
            {
                services.Replace(descriptor);
            }
            else
            {
                services.Add(descriptor);
            }
        }
        else
        {
            // 常规服务注册
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
            if (replaceAttr != null)
            {
                services.Replace(descriptor);
            }
            else
            {
                services.TryAdd(descriptor);
            }
        }
    }
}
