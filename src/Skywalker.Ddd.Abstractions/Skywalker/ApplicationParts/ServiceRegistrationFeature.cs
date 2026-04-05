// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.ApplicationParts;

/// <summary>
/// 服务注册功能，持有从 <see cref="ApplicationPart"/> 中发现的所有 DI 服务描述符。
/// </summary>
public class ServiceRegistrationFeature
{
    /// <summary>
    /// 获取已发现的服务描述符列表（TryAdd 语义）。
    /// </summary>
    public IList<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();

    /// <summary>
    /// 获取需要替换的服务描述符列表（Replace 语义，由 <see cref="ReplaceServiceAttribute"/> 触发）。
    /// </summary>
    public IList<ServiceDescriptor> Replacements { get; } = new List<ServiceDescriptor>();

    /// <summary>
    /// 获取或设置服务集合，供 FeatureProvider 调用外部组件的 AddXxx 注册方法。
    /// </summary>
    public IServiceCollection ServiceCollection { get; set; } = default!;
}
