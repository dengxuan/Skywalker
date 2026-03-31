// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 基于 <see cref="Assembly"/> 的 <see cref="ApplicationPart"/> 实现。
/// </summary>
public class AssemblyPart : ApplicationPart, IApplicationPartTypeProvider
{
    /// <summary>
    /// 初始化 <see cref="AssemblyPart"/> 的新实例。
    /// </summary>
    /// <param name="assembly">支撑此 Part 的程序集。</param>
    public AssemblyPart(Assembly assembly)
    {
        Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    /// <summary>
    /// 获取此 Part 对应的程序集。
    /// </summary>
    public Assembly Assembly { get; }

    /// <inheritdoc />
    public override string Name => Assembly.GetName().Name!;

    /// <inheritdoc />
    public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
}
