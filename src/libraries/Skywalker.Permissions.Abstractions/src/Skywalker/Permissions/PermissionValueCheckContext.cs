// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

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
