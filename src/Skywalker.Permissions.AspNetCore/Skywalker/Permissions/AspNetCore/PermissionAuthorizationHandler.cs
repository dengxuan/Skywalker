// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AspNetCore;

/// <summary>
/// 权限授权处理器
/// 检查当前用户是否拥有指定权限
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionAuthorizationHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 如果用户未认证，直接返回
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        // 检查用户是否拥有该权限（传递 context.User）
        var isGranted = await _permissionChecker.IsGrantedAsync(context.User, requirement.Permission);

        if (isGranted)
        {
            context.Succeed(requirement);
        }
    }
}
