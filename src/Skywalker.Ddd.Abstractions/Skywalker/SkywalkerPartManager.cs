// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
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
    /// 从入口程序集开始，发现所有 Skywalker 相关程序集并添加为 <see cref="AssemblyPart"/>。
    /// </summary>
    /// <param name="entryAssembly">入口程序集。</param>
    /// <remarks>
    /// 优先使用 <see cref="DependencyContext"/> 从 deps.json 中发现所有依赖库（不受编译器优化影响），
    /// 若 <see cref="DependencyContext"/> 不可用则回退到 <see cref="Assembly.GetReferencedAssemblies"/> 递归发现。
    /// </remarks>
    internal void PopulateDefaultParts(Assembly entryAssembly)
    {
        var dependencyContext = DependencyContext.Load(entryAssembly);
        var assemblies = dependencyContext != null
            ? DiscoverFromDependencyContext(entryAssembly, dependencyContext)
            : DiscoverFromReferencedAssemblies(entryAssembly);

        foreach (var assembly in assemblies)
        {
            ApplicationParts.Add(new AssemblyPart(assembly));
        }
    }

    /// <summary>
    /// 通过 <see cref="DependencyContext"/>（deps.json）发现所有 Skywalker 相关程序集。
    /// deps.json 包含完整的依赖图，不受编译器 "未使用引用优化" 的影响。
    /// </summary>
    private static IEnumerable<Assembly> DiscoverFromDependencyContext(Assembly entryAssembly, DependencyContext dependencyContext)
    {
        // 收集所有 Skywalker 库名称（用于判断用户库是否引用了 Skywalker）
        var skywalkerLibNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var lib in dependencyContext.RuntimeLibraries)
        {
            if (lib.Name.StartsWith("Skywalker.", StringComparison.OrdinalIgnoreCase))
            {
                skywalkerLibNames.Add(lib.Name);
            }
        }

        // 入口程序集始终纳入
        yield return entryAssembly;
        var entryName = entryAssembly.GetName().Name;

        foreach (var lib in dependencyContext.RuntimeLibraries)
        {
            // 跳过入口程序集（已 yield）
            if (string.Equals(lib.Name, entryName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!IsSkywalkerRelatedLibrary(lib, skywalkerLibNames))
            {
                continue;
            }

            Assembly? assembly;
            try
            {
                assembly = Assembly.Load(new AssemblyName(lib.Name));
            }
            catch
            {
                continue;
            }

            yield return assembly;
        }
    }

    /// <summary>
    /// 判断 <see cref="RuntimeLibrary"/> 是否与 Skywalker 相关。
    /// </summary>
    private static bool IsSkywalkerRelatedLibrary(RuntimeLibrary lib, HashSet<string> skywalkerLibNames)
    {
        // Skywalker 框架自身程序集
        if (lib.Name.StartsWith("Skywalker.", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // 依赖了任何 Skywalker 库的用户程序集
        foreach (var dep in lib.Dependencies)
        {
            if (skywalkerLibNames.Contains(dep.Name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 回退方案：从入口程序集递归遍历 <see cref="Assembly.GetReferencedAssemblies"/>。
    /// 在 <see cref="DependencyContext"/> 不可用时使用（如某些测试宿主）。
    /// 注意：编译器可能优化掉未实际使用的引用，导致部分程序集无法被发现。
    /// </summary>
    private static IEnumerable<Assembly> DiscoverFromReferencedAssemblies(Assembly entryAssembly)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var entryName = entryAssembly.GetName().Name;
        if (entryName != null && seen.Add(entryName))
        {
            yield return entryAssembly;
        }

        foreach (var assembly in WalkReferencedAssemblies(entryAssembly, seen))
        {
            yield return assembly;
        }
    }

    private static IEnumerable<Assembly> WalkReferencedAssemblies(Assembly assembly, HashSet<string> seen)
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

            foreach (var nested in WalkReferencedAssemblies(referencedAssembly, seen))
            {
                yield return nested;
            }
        }
    }

    private static bool IsSkywalkerAssembly(Assembly assembly, string name)
    {
        if (name.StartsWith("Skywalker.", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (assembly.GetCustomAttribute<SkywalkerServicesAttribute>() != null)
        {
            return true;
        }

        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            if (reference.Name != null && reference.Name.StartsWith("Skywalker.", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
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
