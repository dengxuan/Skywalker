using Microsoft.AspNetCore.Authorization;
using Skywalker.Authorization.Permissions;
using Skywalker.Authorization.Permissions.Abstractions;

namespace Skywalker.AspNetCore.Authorization.Permissions;

public class PermissionsRequirementHandler : AuthorizationHandler<PermissionsRequirement>
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionsRequirementHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
    {
        var multiplePermissionGrantResult = await _permissionChecker.IsGrantedAsync(context.User, requirement.PermissionNames);

        if (requirement.RequiresAll ? multiplePermissionGrantResult.AllGranted : multiplePermissionGrantResult.Result.Any(x => x.Value == PermissionGrantResult.Granted))
        {
            context.Succeed(requirement);
        }
    }
}
