using Microsoft.AspNetCore.Authorization;

namespace Skywalker.AspNetCore.PermissionsEvaluator.Permissions;

public class PermissionsRequirement : IAuthorizationRequirement
{
    public string[] PermissionNames { get; }

    public bool RequiresAll { get; }

    public PermissionsRequirement(string[] permissionNames, bool requiresAll)
    {
        permissionNames.NotNull(nameof(permissionNames));

        PermissionNames = permissionNames;
        RequiresAll = requiresAll;
    }

    public override string ToString()
    {
        return $"PermissionsRequirement: {string.Join(", ", PermissionNames)}";
    }
}
