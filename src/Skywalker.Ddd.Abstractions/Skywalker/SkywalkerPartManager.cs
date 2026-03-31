// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.ApplicationParts;

namespace Skywalker;

/// <summary>
/// 管理 Skywalker 应用程序的 Parts 和 Features。
/// </summary>
/// <remarks>
/// 设计参考 ASP.NET Core 的 <c>ApplicationPartManager</c>：
/// <list type="bullet">
/// <item><see cref="ApplicationParts"/> 持有程序集等应用部件</item>
/// <item><see cref="FeatureProviders"/> 持有功能提供者，用于从 Parts 中发现各类功能</item>
/// <item><see cref="PopulateFeature{TFeature}"/> 驱动功能发现流程</item>
/// </list>
/// </remarks>
public class SkywalkerPartManager
{
    /// <summary>
    /// 获取 <see cref="IApplicationFeatureProvider"/> 列表。
    /// </summary>
    public IList<IApplicationFeatureProvider> FeatureProviders { get; } = new List<IApplicationFeatureProvider>();

    /// <summary>
    /// 获取 <see cref="ApplicationPart"/> 列表。
    /// </summary>
    /// <remarks>
    /// 列表中的实例按优先级顺序存储，靠前的 Part 具有更高的优先级。
    /// <see cref="IApplicationFeatureProvider"/> 可以利用此顺序来解决多个 Part 提供相同功能值时的冲突。
    /// </remarks>
    public IList<ApplicationPart> ApplicationParts { get; } = new List<ApplicationPart>();

    /// <summary>
    /// 使用已注册的 <see cref="IApplicationFeatureProvider{TFeature}"/> 填充指定的功能实例。
    /// </summary>
    /// <typeparam name="TFeature">功能类型。</typeparam>
    /// <param name="feature">要填充的功能实例。</param>
    public void PopulateFeature<TFeature>(TFeature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);

        foreach (var provider in FeatureProviders.OfType<IApplicationFeatureProvider<TFeature>>())
        {
            provider.PopulateFeature(ApplicationParts, feature);
        }
    }

    /// <summary>
    /// 从入口程序集开始，递归发现所有 Skywalker 相关程序集并添加为 <see cref="AssemblyPart"/>。
    /// </summary>
    /// <param name="entryAssembly">入口程序集。</param>
    internal void PopulateDefaultParts(Assembly entryAssembly)
    {
        var seenAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var assemblies = GetApplicationPartAssemblies(entryAssembly, seenAssemblies);

        foreach (var assembly in assemblies)
        {
            ApplicationParts.Add(new AssemblyPart(assembly));
        }
    }

    private static IEnumerable<Assembly> GetApplicationPartAssemblies(Assembly entryAssembly, HashSet<string> seen)
    {
        var entryName = entryAssembly.GetName().Name;
        if (entryName != null && seen.Add(entryName))
        {
            yield return entryAssembly;
        }

        foreach (var assembly in DiscoverReferencedAssemblies(entryAssembly, seen))
        {
            yield return assembly;
        }
    }

    private static IEnumerable<Assembly> DiscoverReferencedAssemblies(Assembly assembly, HashSet<string> seen)
    {
        foreach (var referencedName in assembly.GetReferencedAssemblies())
        {
            if (!ShouldProcessReference(referencedName))
            {
                continue;
            }

            Assembly referencedAssembly;
            try
            {
                referencedAssembly = Assembly.Load(referencedName);
            }
            catch
            {
                continue;
            }

            var name = referencedAssembly.GetName().Name;
            if (name == null || !seen.Add(name))
            {
                continue;
            }

            if (IsSkywalkerAssembly(referencedAssembly, name))
            {
                yield return referencedAssembly;
            }

            // 递归发现
            foreach (var nested in DiscoverReferencedAssemblies(referencedAssembly, seen))
            {
                yield return nested;
            }
        }
    }

    private static bool IsSkywalkerAssembly(Assembly assembly, string name)
    {
        return assembly.GetCustomAttribute<SkywalkerServicesAttribute>() != null ||
               name.StartsWith("Skywalker.", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldProcessReference(AssemblyName referencedName)
    {
        var name = referencedName.Name;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        if (name.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (name.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) &&
            !name.StartsWith("Microsoft.Extensions", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}
