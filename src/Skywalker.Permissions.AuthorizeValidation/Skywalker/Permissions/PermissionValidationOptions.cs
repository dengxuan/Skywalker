// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class PermissionValidationOptions
{
    /// <summary>
    /// 权限服务端地址
    /// </summary>
    public string Authority { get; set; } = "https://localhost";

    /// <summary>
    /// 权限授权同步端点（从 Discovery 自动获取或手动配置）
    /// </summary>
    public string PermissionGrantsEndpoint { get; set; } = "/_permissions/grants";

    /// <summary>
    /// 服务间认证密钥
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// 刷新间隔
    /// </summary>
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromMinutes(5);
}
