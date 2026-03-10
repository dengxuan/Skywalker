// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

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
