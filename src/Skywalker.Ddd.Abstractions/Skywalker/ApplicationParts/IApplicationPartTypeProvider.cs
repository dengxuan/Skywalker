// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Reflection;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 暴露 <see cref="ApplicationPart"/> 中可用类型的接口。
/// </summary>
public interface IApplicationPartTypeProvider
{
    /// <summary>
    /// 获取此 Part 中的所有可用类型。
    /// </summary>
    IEnumerable<TypeInfo> Types { get; }
}
