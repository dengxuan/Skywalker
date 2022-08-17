// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class PermissionDefinitionContext
{
    private readonly List<PermissionDefinition> _roots = new();

    public IReadOnlyList<PermissionDefinition> Permissions => _roots;

    public PermissionDefinition AddPermission(string name, string displayName, bool isEnabled = true)
    {
        var permission = new PermissionDefinition(name, displayName, isEnabled);

        _roots.Add(permission);

        return permission;
    }
}
