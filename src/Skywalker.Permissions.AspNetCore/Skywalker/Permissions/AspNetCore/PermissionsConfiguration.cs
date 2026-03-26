// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限服务发现配置
/// </summary>
public class PermissionsConfiguration
{
    /// <summary>
    /// 权限授权同步端点
    /// </summary>
    [JsonPropertyName("permission_grants_endpoint")]
    public string PermissionGrantsEndpoint { get; set; } = "/permissions/grants";

    /// <summary>
    /// 权限注册端点
    /// </summary>
    [JsonPropertyName("register_endpoint")]
    public string RegisterEndpoint { get; set; } = "/permissions/register";
}
