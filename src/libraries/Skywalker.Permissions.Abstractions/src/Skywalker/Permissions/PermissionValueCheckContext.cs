using System.Security.Claims;

namespace Skywalker.Permissions;

public class PermissionValueCheckContext
{
    public PermissionDefinition Permission { get; }

    public ClaimsPrincipal? Principal { get; }

    public PermissionValueCheckContext(PermissionDefinition permission, ClaimsPrincipal? principal)
    {
        permission.NotNull(nameof(permission));

        Permission = permission;
        Principal = principal;
    }
}
