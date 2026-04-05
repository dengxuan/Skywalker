// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 共享的服务注册辅助方法，处理 <see cref="ExposeServicesAttribute"/>、
/// <see cref="ReplaceServiceAttribute"/>、<see cref="SharedInstanceAttribute"/>、
/// <see cref="KeyedServiceAttribute"/> 等约定。
/// </summary>
public static class ServiceRegistrationHelper
{
    /// <summary>
    /// 处理类型上的 <see cref="KeyedServiceAttribute"/>，注册为 Keyed Service。
    /// 此方法独立于 <see cref="RegisterType"/>。
    /// </summary>
    public static void RegisterKeyedServices(Type type, ServiceRegistrationFeature feature)
    {
        var keyedAttrs = type.GetCustomAttributes<KeyedServiceAttribute>(false);
        var isReplace = type.IsDefined(typeof(ReplaceServiceAttribute), true);
        var target = isReplace ? feature.Replacements : feature.Services;

        foreach (var keyed in keyedAttrs)
        {
            target.Add(ServiceDescriptor.KeyedScoped(keyed.ServiceType, keyed.Key, type));
        }
    }

    /// <summary>
    /// 根据类型上的注册 Attribute 注册服务到 Feature。
    /// 处理 <see cref="ExposeServicesAttribute"/>、<see cref="ReplaceServiceAttribute"/>、
    /// <see cref="SharedInstanceAttribute"/> 等约定。
    /// </summary>
    /// <param name="type">实现类型。</param>
    /// <param name="defaultInterfaces">自动发现的接口集合（已排除基础接口）。</param>
    /// <param name="lifetime">服务生命周期。</param>
    /// <param name="feature">注册目标。</param>
    public static void RegisterType(
        Type type,
        IEnumerable<Type> defaultInterfaces,
        ServiceLifetime lifetime,
        ServiceRegistrationFeature feature)
    {
        var isReplace = type.IsDefined(typeof(ReplaceServiceAttribute), true);
        var sharedInstance = type.IsDefined(typeof(SharedInstanceAttribute), false);
        var exposeAttr = type.GetCustomAttribute<ExposeServicesAttribute>(true);
        var target = isReplace ? feature.Replacements : feature.Services;

        // 确定要注册的接口列表
        var interfaces = ResolveServiceTypes(type, defaultInterfaces, exposeAttr);

        if (sharedInstance)
        {
            // 共享实例模式：先注册实现类自身，然后各接口通过工厂委托解析同一实例
            target.Add(new ServiceDescriptor(type, type, lifetime));
            foreach (var iface in interfaces)
            {
                target.Add(new ServiceDescriptor(iface, sp => sp.GetRequiredService(type), lifetime));
            }
        }
        else
        {
            // 标准模式
            foreach (var iface in interfaces)
            {
                target.Add(new ServiceDescriptor(iface, type, lifetime));
            }
        }

        // IncludeSelf
        if (exposeAttr is { IncludeSelf: true } && !sharedInstance)
        {
            target.Add(new ServiceDescriptor(type, type, lifetime));
        }
    }

    private static IEnumerable<Type> ResolveServiceTypes(
        Type type,
        IEnumerable<Type> defaultInterfaces,
        ExposeServicesAttribute? exposeAttr)
    {
        if (exposeAttr == null)
        {
            // 无 [ExposeServices]：使用自动发现的接口
            return defaultInterfaces;
        }

        // 有 [ExposeServices]：使用显式指定的接口
        var result = new List<Type>(exposeAttr.ServiceTypes);

        if (exposeAttr.IncludeDefaults)
        {
            // 同时包含自动发现的接口
            foreach (var iface in defaultInterfaces)
            {
                if (!result.Contains(iface))
                {
                    result.Add(iface);
                }
            }
        }

        return result;
    }
}
