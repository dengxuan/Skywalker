// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions.Abstractions;

public interface IPermissionDefinitionManager
{
    Task<PermissionDefinition> GetAsync(string name);

    Task<PermissionDefinition?> GetOrNullAsync(string name);

    Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync();
    
    Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync(params string[] names);

    Task CreatePermissionsAsync(IReadOnlyList<PermissionDefinition> permissionDefinitions);
}
