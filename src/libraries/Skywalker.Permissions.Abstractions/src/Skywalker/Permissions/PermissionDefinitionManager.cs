// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.Localization;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public abstract class PermissionDefinitionManager : IPermissionDefinitionManager
{

    protected static PermissionDefinition CreatePermission(string name, string localizedStringName, bool isEnabled = true)
    {
        var displayName = new LocalizedString(localizedStringName, localizedStringName);
        return new PermissionDefinition(name, displayName, isEnabled);
    }

    public abstract Task<PermissionDefinition> GetAsync(string name);

    public abstract Task<PermissionDefinition?> GetOrNullAsync(string name);

    public abstract Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync();
    
    public abstract Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync(params string[] names);
}
