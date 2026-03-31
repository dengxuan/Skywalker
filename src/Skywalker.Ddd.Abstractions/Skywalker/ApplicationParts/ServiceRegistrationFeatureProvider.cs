// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 从 <see cref="ApplicationPart"/> 中发现标记了 DI 接口的类型，生成 <see cref="ServiceDescriptor"/>。
/// </summary>
public class ServiceRegistrationFeatureProvider : IApplicationFeatureProvider<ServiceRegistrationFeature>
{
    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ServiceRegistrationFeature feature)
    {
        foreach (var part in parts)
        {
            if (part is not IApplicationPartTypeProvider typeProvider)
            {
                continue;
            }

            foreach (var typeInfo in typeProvider.Types)
            {
                var type = typeInfo.AsType();

                if (!type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (type.IsDefined(typeof(DisableAutoRegistrationAttribute), false))
                {
                    continue;
                }

                var lifetime = GetServiceLifetime(type);
                if (lifetime == null)
                {
                    continue;
                }

                foreach (var serviceType in GetServiceTypes(type))
                {
                    feature.Services.Add(CreateDescriptor(serviceType, type, lifetime.Value));
                }
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

        var interfaces = implementationType.GetInterfaces()
            .Where(i => !IsMarkerInterface(i))
            .Where(i => !i.IsGenericType || !IsGenericMarkerInterface(i.GetGenericTypeDefinition()));

        foreach (var iface in interfaces)
        {
            yield return iface;
        }

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
        return false;
    }

    private static ServiceDescriptor CreateDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        var keyedAttr = implementationType.GetCustomAttribute<KeyedServiceAttribute>();
        if (keyedAttr != null)
        {
            return new ServiceDescriptor(serviceType, keyedAttr.Key, implementationType, lifetime);
        }
        return new ServiceDescriptor(serviceType, implementationType, lifetime);
    }
}
