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
    /// 获取已发现的服务描述符列表。
    /// </summary>
    public IList<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();
}
