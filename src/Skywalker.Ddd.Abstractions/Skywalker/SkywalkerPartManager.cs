// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.DependencyInjection;

namespace Skywalker;

/// <summary>
/// Skywalker 程序集管理器，用于发现和注册服务。
/// </summary>
public sealed class SkywalkerPartManager
{
    private readonly HashSet<string> _processedAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Assembly> _skywalkerAssemblies = [];

    /// <summary>
    /// 获取已发现的 Skywalker 程序集列表。
    /// </summary>
    public IReadOnlyList<Assembly> Assemblies => _skywalkerAssemblies;

    /// <summary>
    /// 从入口程序集开始，发现所有包含 <see cref="SkywalkerServicesAttribute"/> 特性的程序集。
    /// </summary>
    /// <param name="entryAssembly">入口程序集，默认使用 <see cref="Assembly.GetEntryAssembly"/>。</param>
    public void DiscoverAssemblies(Assembly? entryAssembly = null)
    {
        entryAssembly ??= Assembly.GetEntryAssembly();
        if (entryAssembly == null)
        {
            return;
        }

        // 入口程序集始终包含在扫描范围内
        var entryName = entryAssembly.GetName().Name;
        if (entryName != null && _processedAssemblies.Add(entryName))
        {
            _skywalkerAssemblies.Add(entryAssembly);
        }

        // 递归发现引用的 Skywalker 程序集
        foreach (var referencedName in entryAssembly.GetReferencedAssemblies())
        {
            if (!ShouldProcessReference(referencedName))
            {
                continue;
            }

            try
            {
                var referencedAssembly = Assembly.Load(referencedName);
                DiscoverAssembliesRecursive(referencedAssembly);
            }
            catch
            {
                // 忽略无法加载的程序集
            }
        }
    }

    private void DiscoverAssembliesRecursive(Assembly assembly)
    {
        var name = assembly.GetName().Name;
        if (name == null || !_processedAssemblies.Add(name))
        {
            return;
        }

        // 检查是否有 SkywalkerServicesAttribute 特性 或 是 DDD 内部模块（按命名约定）
        if (assembly.GetCustomAttribute<SkywalkerServicesAttribute>() != null || IsDddAssembly(name))
        {
            _skywalkerAssemblies.Add(assembly);
        }

        // 递归处理引用的程序集
        foreach (var referencedName in assembly.GetReferencedAssemblies())
        {
            if (!ShouldProcessReference(referencedName))
            {
                continue;
            }

            try
            {
                var referencedAssembly = Assembly.Load(referencedName);
                DiscoverAssembliesRecursive(referencedAssembly);
            }
            catch
            {
                // 忽略无法加载的程序集
            }
        }
    }

    private static bool ShouldProcessReference(AssemblyName referencedName)
    {
        var name = referencedName.Name;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        // 跳过系统程序集
        if (name.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // 跳过 Microsoft 程序集（但保留 Microsoft.Extensions）
        if (name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
            !name.StartsWith("Microsoft.Extensions", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断程序集是否为 DDD 内部模块（按命名约定自动发现）。
    /// </summary>
    private static bool IsDddAssembly(string? assemblyName)
    {
        return assemblyName != null && assemblyName.StartsWith("Skywalker.Ddd.", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 从所有已发现程序集中扫描并注册服务（基于反射）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public void RegisterAllServices(IServiceCollection services)
    {
        foreach (var assembly in _skywalkerAssemblies)
        {
            // 使用反射扫描注册服务
            ServiceRegistrar.RegisterAssembly(services, assembly);
        }
    }
}
