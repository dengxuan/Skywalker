using Microsoft.Extensions.Localization;
using Skywalker.Extensions.Exceptions;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions;

public class PermissionDefinitionContext : IPermissionDefinitionContext
{
    public IServiceProvider ServiceProvider { get; }

    public Dictionary<string, PermissionGroupDefinition> Groups { get; }

    public PermissionDefinitionContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Groups = new Dictionary<string, PermissionGroupDefinition>();
    }

    public virtual PermissionGroupDefinition AddGroup(string name, LocalizedString? displayName = null)
    {
        name.NotNull(nameof(name));

        if (Groups.ContainsKey(name))
        {
            throw new SkywalkerException($"There is already an existing permission group with name: {name}");
        }

        return Groups[name] = new PermissionGroupDefinition(name, displayName);
    }

    public virtual PermissionGroupDefinition GetGroup(string name)
    {
        var group = GetGroupOrNull(name);

        if (group == null)
        {
            throw new SkywalkerException($"Could not find a permission definition group with the given name: {name}");
        }

        return group;
    }

    public virtual PermissionGroupDefinition? GetGroupOrNull(string name)
    {
        name.NotNull(nameof(name));

        if (!Groups.ContainsKey(name))
        {
            return null;
        }

        return Groups[name];
    }

    public virtual void RemoveGroup(string name)
    {
        name.NotNull(nameof(name));

        if (!Groups.ContainsKey(name))
        {
            throw new SkywalkerException($"Not found permission group with name: {name}");
        }

        Groups.Remove(name);
    }

    public virtual PermissionDefinition? GetPermissionOrNull(string name)
    {
        name.NotNull(nameof(name));

        foreach (var groupDefinition in Groups.Values)
        {
            var permissionDefinition = groupDefinition.GetPermissionOrNull(name);

            if (permissionDefinition != null)
            {
                return permissionDefinition;
            }
        }

        return null;
    }
}
