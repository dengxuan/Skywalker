// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker;

/// <summary>
/// Skywalker 服务构建器，用于链式注册各模块服务。
/// </summary>
/// <remarks>
/// 使用示例：
/// <code>
/// services.AddSkywalker()
///     .AddEntityFramework&lt;MyDbContext&gt;(options => options.UseSqlServer(...));
/// </code>
/// </remarks>
public interface ISkywalkerBuilder
{
    /// <summary>
    /// 底层服务集合。
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// 程序集管理器，用于发现和注册服务。
    /// </summary>
    SkywalkerPartManager PartManager { get; }
}
