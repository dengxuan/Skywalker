using System;
using System.Collections.Generic;

namespace Skywalker.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// 表示一个 Skywalker 模块的信息。
/// </summary>
internal sealed class ModuleInfo : IEquatable<ModuleInfo>
{
    /// <summary>
    /// 获取程序集名称。
    /// </summary>
    public string AssemblyName { get; }

    /// <summary>
    /// 获取扩展类的完全限定名称。
    /// </summary>
    public string ExtensionClassFullName { get; }

    /// <summary>
    /// 获取扩展方法名称（如 AddDddCore）。
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// 获取程序集依赖列表。
    /// </summary>
    public IReadOnlyList<string> Dependencies { get; }

    /// <summary>
    /// 初始化 <see cref="ModuleInfo"/> 类的新实例。
    /// </summary>
    public ModuleInfo(
        string assemblyName,
        string extensionClassFullName,
        string methodName,
        IReadOnlyList<string> dependencies)
    {
        AssemblyName = assemblyName;
        ExtensionClassFullName = extensionClassFullName;
        MethodName = methodName;
        Dependencies = dependencies;
    }

    public bool Equals(ModuleInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AssemblyName == other.AssemblyName;
    }

    public override bool Equals(object? obj) => Equals(obj as ModuleInfo);

    public override int GetHashCode() => AssemblyName.GetHashCode();

    public override string ToString() => $"{AssemblyName} -> {MethodName}";
}
