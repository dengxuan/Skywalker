// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker;

/// <summary>
/// Skywalker 服务构建器，用于链式注册各模块服务。
/// </summary>
public interface ISkywalkerBuilder
{
    /// <summary>
    /// 底层服务集合。
    /// </summary>
    IServiceCollection Services { get; }
}
