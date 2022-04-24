﻿using Microsoft.AspNetCore.Authorization;
using Skywalker.Authorization.Permissions.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.AspNetCore.Authorization.Permissions;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>, ISingletonDependency
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionRequirementHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (await _permissionChecker.IsGrantedAsync(context.User, requirement.PermissionName))
        {
            context.Succeed(requirement);
        }
    }
}
