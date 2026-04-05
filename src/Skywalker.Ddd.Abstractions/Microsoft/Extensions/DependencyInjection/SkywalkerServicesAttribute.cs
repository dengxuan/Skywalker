// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标记程序集包含 Skywalker 服务，需要被 <see cref="Skywalker.SkywalkerPartManager"/> 发现。
/// </summary>
/// <remarks>
/// 标记此特性的程序集会在 <c>AddSkywalker()</c> 调用时被自动发现并添加为 <see cref="Skywalker.ApplicationParts.AssemblyPart"/>。
/// 通常不需要手动标记——引用了 <c>Skywalker.*</c> 程序集的项目会自动被发现。
/// 仅在程序集不直接引用 Skywalker 但仍需参与服务发现时使用。
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class SkywalkerServicesAttribute : Attribute
{
}
