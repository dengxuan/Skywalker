using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.PermissionsEvaluator.Permissions;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public PermissionRequirement(string permissionName)
    {
        permissionName.NotNull(nameof(permissionName));

        PermissionName = permissionName;
    }

    public override string ToString()
    {
        return $"PermissionRequirement: {PermissionName}";
    }
}
