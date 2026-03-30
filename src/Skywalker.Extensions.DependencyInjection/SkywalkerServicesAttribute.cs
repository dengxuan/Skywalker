// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.DependencyInjection;

/// <summary>
/// 标记程序集包含自动服务注册扩展类。
/// 由 SourceGenerator 自动生成此特性。
/// </summary>
/// <remarks>
/// 此特性用于 <c>AddSkywalker()</c> 在运行时发现所有需要注册的程序集。
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
