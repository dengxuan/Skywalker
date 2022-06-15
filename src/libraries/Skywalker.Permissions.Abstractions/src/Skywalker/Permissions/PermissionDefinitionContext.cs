// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.Extensions.Localization;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class PermissionDefinitionContext : IPermissionDefinitionContext
{

    private readonly List<PermissionDefinition> _permissions;
    public IReadOnlyList<PermissionDefinition> Permissions => _permissions.ToImmutableList();

    public PermissionDefinitionContext()
    {
        _permissions = new List<PermissionDefinition>();
    }

    public virtual PermissionDefinition AddPermission(string name, LocalizedString? displayName = null, bool isEnabled = true)
    {
        var permission = new PermissionDefinition(name, displayName, isEnabled);

        _permissions.Add(permission);

        return permission;
    }


    public virtual PermissionDefinition? FindPermission(string name)
    {
        return _permissions.FirstOrDefault(predicate => predicate.Name == name);
    }
}
