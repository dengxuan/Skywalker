// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限授权存储接口，Server 端专用，用于提供同步数据
/// 由使用方（如 Gaming 项目）实现，查询数据库
/// </summary>
public interface IPermissionGrantStore
{
    /// <summary>
    /// 获取所有授权数据
    /// </summary>
    Task<IReadOnlyList<PermissionGrantInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据版本号，用于 ETag
    /// </summary>
    Task<string> GetVersionAsync(CancellationToken cancellationToken = default);
}
