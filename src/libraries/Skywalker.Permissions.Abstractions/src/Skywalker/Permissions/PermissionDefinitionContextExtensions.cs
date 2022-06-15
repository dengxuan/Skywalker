// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public static class PermissionDefinitionContextExtensions
{
    /// <summary>
    /// Finds and disables a permission with the given <paramref name="name"/>.
    /// Returns false if given permission was not found.
    /// </summary>
    /// <param name="context">Permission definition context</param>
    /// <param name="name">Name of the permission</param>
    /// <returns>
    /// Returns true if given permission was found.
    /// Returns false if given permission was not found.
    /// </returns>
    public static bool TryDisablePermission(this IPermissionDefinitionContext context, string name)
    {
        context.NotNull(nameof(context));
        name.NotNull(nameof(name));

        var permission = context.FindPermission(name);
        if (permission == null)
        {
            return false;
        }

        permission.IsEnabled = false;
        return true;
    }
}
