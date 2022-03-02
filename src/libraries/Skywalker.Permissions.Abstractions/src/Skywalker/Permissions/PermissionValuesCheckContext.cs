using System.Security.Claims;

namespace Skywalker.Permissions;

public class PermissionValuesCheckContext
{
    public List<PermissionDefinition> Permissions { get; }

    public ClaimsPrincipal? Principal { get; }

    public PermissionValuesCheckContext(List<PermissionDefinition> permissions, ClaimsPrincipal? principal)
    {
        permissions.NotNull(nameof(permissions));

        Permissions = permissions;
        Principal = principal;
    }
}
