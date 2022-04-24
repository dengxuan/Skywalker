﻿using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Authorization.Permissions.Abstractions;
using Skywalker.ExceptionHandler;

namespace Skywalker.Authorization.Permissions;

public class PermissionDefinitionManager : IPermissionDefinitionManager
{
    protected IDictionary<string, PermissionGroupDefinition> PermissionGroupDefinitions => _lazyPermissionGroupDefinitions.Value;
    private readonly Lazy<Dictionary<string, PermissionGroupDefinition>> _lazyPermissionGroupDefinitions;

    protected IDictionary<string, PermissionDefinition> PermissionDefinitions => _lazyPermissionDefinitions.Value;
    private readonly Lazy<Dictionary<string, PermissionDefinition>> _lazyPermissionDefinitions;

    protected PermissionOptions Options { get; }

    private readonly IServiceProvider _serviceProvider;

    public PermissionDefinitionManager(IOptions<PermissionOptions> options, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Options = options.Value;

        _lazyPermissionDefinitions = new Lazy<Dictionary<string, PermissionDefinition>>(
            CreatePermissionDefinitions,
            isThreadSafe: true
        );

        _lazyPermissionGroupDefinitions = new Lazy<Dictionary<string, PermissionGroupDefinition>>(
            CreatePermissionGroupDefinitions,
            isThreadSafe: true
        );
    }

    public virtual PermissionDefinition Get(string name)
    {
        var permission = GetOrNull(name);

        if (permission == null)
        {
            throw new SkywalkerException("Undefined permission: " + name);
        }

        return permission;
    }

    public virtual PermissionDefinition? GetOrNull(string name)
    {
        name.NotNull(nameof(name));

        return PermissionDefinitions.GetOrDefault(name);
    }

    public virtual IReadOnlyList<PermissionDefinition> GetPermissions()
    {
        return PermissionDefinitions.Values.ToImmutableList();
    }

    public IReadOnlyList<PermissionGroupDefinition> GetGroups()
    {
        return PermissionGroupDefinitions.Values.ToImmutableList();
    }

    protected virtual Dictionary<string, PermissionDefinition> CreatePermissionDefinitions()
    {
        var permissions = new Dictionary<string, PermissionDefinition>();

        foreach (var groupDefinition in PermissionGroupDefinitions.Values)
        {
            foreach (var permission in groupDefinition.Permissions)
            {
                AddPermissionToDictionaryRecursively(permissions, permission);
            }
        }

        return permissions;
    }

    protected virtual void AddPermissionToDictionaryRecursively(Dictionary<string, PermissionDefinition> permissions, PermissionDefinition permission)
    {
        if (permissions.ContainsKey(permission.Name))
        {
            throw new SkywalkerException("Duplicate permission name: " + permission.Name);
        }

        permissions[permission.Name] = permission;

        foreach (var child in permission.Children)
        {
            AddPermissionToDictionaryRecursively(permissions, child);
        }
    }

    protected virtual Dictionary<string, PermissionGroupDefinition> CreatePermissionGroupDefinitions()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = new PermissionDefinitionContext(scope.ServiceProvider);

            var providers = Options.DefinitionProviders
                                   .Select(p => scope.ServiceProvider.GetRequiredService(p) as IPermissionDefinitionProvider)
                                   .ToList();

            foreach (var provider in providers)
            {
                provider?.PreDefine(context);
            }

            foreach (var provider in providers)
            {
                provider?.Define(context);
            }

            foreach (var provider in providers)
            {
                provider?.PostDefine(context);
            }

            return context.Groups;
        }
    }
}
