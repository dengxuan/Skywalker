// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限验证特性
/// </summary>
/// <remarks>
/// 使用示例：
/// [RequirePermission("Gaming.Identity.Users.Create")]
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 权限策略前缀
    /// </summary>
    public const string PolicyPrefix = "Permission:";

    /// <summary>
    /// 权限名称
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permission">权限名称</param>
    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
        Policy = $"{PolicyPrefix}{permission}";
    }
}
