// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限服务端配置选项
/// </summary>
public class PermissionServerOptions
{
    /// <summary>
    /// 服务间认证密钥配置，Key 为系统名称，Value 为密钥
    /// </summary>
    public Dictionary<string, string> Secrets { get; } = new();

    /// <summary>
    /// 发现端点路径
    /// </summary>
    public string DiscoveryEndpoint { get; set; } = "/.well-known/permissions-configuration";

    /// <summary>
    /// 权限授权同步端点
    /// </summary>
    public string PermissionGrantsEndpoint { get; set; } = "/_permissions/grants";

    /// <summary>
    /// 权限注册端点
    /// </summary>
    public string RegisterEndpoint { get; set; } = "/_permissions/register";
}
