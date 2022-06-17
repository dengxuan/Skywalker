// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AuthorizeValidation;

internal class RemotePermissionDefinitionManager : IPermissionDefinitionManager
{
    public Task CreatePermissionsAsync(IReadOnlyList<PermissionDefinition> permissionDefinitions) => throw new NotImplementedException();
    public Task<PermissionDefinition> GetAsync(string name) => throw new NotImplementedException();
    public Task<PermissionDefinition?> GetOrNullAsync(string name) => throw new NotImplementedException();
    public Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync() => throw new NotImplementedException();
    public Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync(params string[] names) => throw new NotImplementedException();
}
