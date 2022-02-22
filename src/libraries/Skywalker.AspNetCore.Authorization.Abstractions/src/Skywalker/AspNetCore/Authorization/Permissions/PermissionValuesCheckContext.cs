using System.Collections.Generic;
using System.Security.Claims;
using JetBrains.Annotations;

namespace Skywalker.Authorization.Permissions;

public class PermissionValuesCheckContext
{
    [NotNull]
    public List<PermissionDefinition> Permissions { get; }

    [CanBeNull]
    public ClaimsPrincipal Principal { get; }

    public PermissionValuesCheckContext(
        [NotNull] List<PermissionDefinition> permissions,
        [CanBeNull] ClaimsPrincipal principal)
    {
        permissions.NotNull(nameof(permissions));

        Permissions = permissions;
        Principal = principal;
    }
}
