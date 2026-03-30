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

        DiscoverAssembliesRecursive(entryAssembly);
    }

    private void DiscoverAssembliesRecursive(Assembly assembly)
    {
        var name = assembly.GetName().Name;
        if (name == null || !_processedAssemblies.Add(name))
        {
            return;
        }

        // 检查是否有 SkywalkerServicesAttribute 特性
        if (assembly.GetCustomAttribute<SkywalkerServicesAttribute>() != null)
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
    /// 调用所有已发现程序集的服务注册方法。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public void RegisterAllServices(IServiceCollection services)
    {
        foreach (var assembly in _skywalkerAssemblies)
        {
            var attr = assembly.GetCustomAttribute<SkywalkerServicesAttribute>();
            if (attr == null)
            {
                continue;
            }

            var method = attr.RegistrationType.GetMethod(
                "AddAutoServices",
                BindingFlags.Public | BindingFlags.Static,
                null,
                [typeof(IServiceCollection)],
                null);

            method?.Invoke(null, [services]);
        }
    }
}
