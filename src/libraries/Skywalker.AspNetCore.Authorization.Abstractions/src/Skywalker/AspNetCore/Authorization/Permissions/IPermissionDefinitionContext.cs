﻿using Microsoft.Extensions.Localization;

namespace Skywalker.AspNetCore.Authorization.Permissions;

public interface IPermissionDefinitionContext
{
    //TODO: Add Get methods to find and modify a permission or group.

    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a pre-defined permission group.
    /// Throws <see cref="AbpException"/> if can not find the given group.
    /// </summary>
    /// <param name="name">Name of the group</param>
    /// <returns></returns>
    PermissionGroupDefinition GetGroup(string name);

    /// <summary>
    /// Tries to get a pre-defined permission group.
    /// Returns null if can not find the given group.
    /// </summary>
    /// <param name="name">Name of the group</param>
    /// <returns></returns>
    PermissionGroupDefinition? GetGroupOrNull(string name);

    /// <summary>
    /// Tries to add a new permission group.
    /// Throws <see cref="AbpException"/> if there is a group with the name.
    /// <param name="name">Name of the group</param>
    /// <param name="displayName">Localized display name of the group</param>
    /// </summary>
    PermissionGroupDefinition AddGroup(string name, LocalizedString? displayName = null);

    /// <summary>
    /// Tries to remove a permission group.
    /// Throws <see cref="AbpException"/> if there is not any group with the name.
    /// <param name="name">Name of the group</param>
    /// </summary>
    void RemoveGroup(string name);

    /// <summary>
    /// Tries to get a pre-defined permission group.
    /// Returns null if can not find the given group.
    /// <param name="name">Name of the group</param>
    /// </summary>
    PermissionDefinition? GetPermissionOrNull(string name);
}
