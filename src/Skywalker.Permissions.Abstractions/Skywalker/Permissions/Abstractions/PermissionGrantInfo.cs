// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions.Abstractions;

/// <summary>
/// 权限授权信息
/// </summary>
public class PermissionGrantInfo
{
    /// <summary>
    /// 权限名称
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 提供者名称：U (User), R (Role), C (Client)
    /// </summary>
    public required string ProviderName { get; set; }

    /// <summary>
    /// 提供者键
    /// </summary>
    public required string ProviderKey { get; set; }
}
