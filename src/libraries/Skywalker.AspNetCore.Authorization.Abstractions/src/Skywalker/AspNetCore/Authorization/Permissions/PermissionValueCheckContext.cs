using System.Security.Claims;
using JetBrains.Annotations;

namespace Skywalker.Authorization.Permissions;

public class PermissionValueCheckContext
{
    [NotNull]
    public PermissionDefinition Permission { get; }

    [CanBeNull]
    public ClaimsPrincipal Principal { get; }

    public PermissionValueCheckContext(
        [NotNull] PermissionDefinition permission,
        [CanBeNull] ClaimsPrincipal principal)
    {
        permission.NotNull(nameof(permission));

        Permission = permission;
        Principal = principal;
    }
}
