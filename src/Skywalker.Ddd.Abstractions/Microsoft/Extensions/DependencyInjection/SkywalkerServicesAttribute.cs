// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标记程序集包含 Skywalker 服务，需要被 <see cref="Skywalker.SkywalkerPartManager"/> 发现。
/// </summary>
/// <remarks>
/// 标记此特性的程序集会在 <c>AddSkywalker()</c> 调用时被自动发现并添加为 <see cref="Skywalker.ApplicationParts.AssemblyPart"/>。
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class SkywalkerServicesAttribute : Attribute
{
    /// <summary>
    /// 获取包含 AddAutoServices 方法的扩展类类型。
    /// </summary>
    public Type RegistrationType { get; }

    /// <summary>
    /// 初始化 <see cref="SkywalkerServicesAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="registrationType">包含 AddAutoServices 方法的类型。</param>
    public SkywalkerServicesAttribute(Type registrationType)
    {
        RegistrationType = registrationType ?? throw new ArgumentNullException(nameof(registrationType));
    }
}
