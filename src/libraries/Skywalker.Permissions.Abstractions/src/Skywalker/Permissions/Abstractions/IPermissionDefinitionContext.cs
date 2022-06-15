// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Localization;

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionDefinitionContext
{
    /// <summary>
    /// Adds a new permission definition to the context.
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <param name="displayName">Permission DisplayName</param>
    /// <param name="isEnabled">True if permission is enable otherwise false</param>
    /// <returns>An instance of <see cref="PermissionDefinition"/></returns>
    PermissionDefinition AddPermission(string name, LocalizedString? displayName = null, bool isEnabled = true);

    /// <summary>
    /// Tries to get a pre-defined permissions.
    /// Returns null if can not find the given permission.
    /// </summary>
    /// <param name="name">Name of the permissions</param>
    /// <returns>An instance of <see cref="PermissionDefinition"/> if it's found, otherwise null.</returns>
    PermissionDefinition? FindPermission(string name);
}
