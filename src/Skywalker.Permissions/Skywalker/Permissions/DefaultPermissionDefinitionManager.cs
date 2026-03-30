// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

/// <summary>
/// 默认权限定义管理器，使用内存存储
/// 启动时自动从 PermissionOptions.DefinitionProviders 加载权限定义
/// </summary>
public class DefaultPermissionDefinitionManager : IPermissionDefinitionManager
{
    private readonly Dictionary<string, PermissionDefinition> _permissions = new();
    private readonly object _lock = new();
    private bool _initialized;

    private readonly IServiceProvider _serviceProvider;
    private readonly PermissionOptions _options;

    public DefaultPermissionDefinitionManager(
        IServiceProvider serviceProvider,
        IOptions<PermissionOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (_lock)
        {
            if (_initialized)
            {
                return;
            }

            foreach (var providerType in _options.DefinitionProviders)
            {
                var provider = (IPermissionDefinitionProvider)ActivatorUtilities.CreateInstance(_serviceProvider, providerType);
                var context = new PermissionDefinitionContext();
                provider.Define(context);

                foreach (var permission in context.Permissions)
                {
                    AddPermissionRecursive(permission);
                }
            }

            _initialized = true;
        }
    }

    private void AddPermissionRecursive(PermissionDefinition permission)
    {
        _permissions[permission.Name] = permission;
        foreach (var child in permission.Children)
        {
            AddPermissionRecursive(child);
        }
    }

    public Task<PermissionDefinition> GetAsync(string name)
    {
        EnsureInitialized();
        var permission = GetOrNullAsync(name).Result;
        if (permission == null)
        {
            throw new KeyNotFoundException($"Permission '{name}' not found.");
        }
        return Task.FromResult(permission);
    }

    public Task<PermissionDefinition?> GetOrNullAsync(string name)
    {
        EnsureInitialized();
        lock (_lock)
        {
            _permissions.TryGetValue(name, out var permission);
            return Task.FromResult(permission);
        }
    }

    public Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
        EnsureInitialized();
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<PermissionDefinition>>(_permissions.Values.ToList());
        }
    }

    public Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync(params string[] names)
    {
        EnsureInitialized();
        lock (_lock)
        {
            var result = names
                .Where(name => _permissions.ContainsKey(name))
                .Select(name => _permissions[name])
                .ToList();
            return Task.FromResult<IReadOnlyList<PermissionDefinition>>(result);
        }
    }

    public Task CreatePermissionsAsync(IReadOnlyList<PermissionDefinition> permissionDefinitions)
    {
        lock (_lock)
        {
            foreach (var definition in permissionDefinitions)
            {
                AddPermissionRecursive(definition);
            }
        }
        return Task.CompletedTask;
    }
}
