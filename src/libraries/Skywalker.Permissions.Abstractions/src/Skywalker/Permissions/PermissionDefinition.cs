// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json.Serialization;
#endif
using Microsoft.Extensions.Localization;
#if NETSTANDARD
using Newtonsoft.Json;
#endif
using Skywalker.Extensions.SimpleStateChecking;

namespace Skywalker.Permissions;

public class PermissionDefinition : IHasSimpleStateCheckers<PermissionDefinition>
{
    /// <summary>
    /// Unique name of the permission.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Parent of this permission if one exists.
    /// If set, this permission can be granted only if parent is granted.
    /// </summary>
    [JsonIgnore]
    public PermissionDefinition? Parent { get; set; }

    /// <summary>
    /// A list of allowed providers to get/set value of this permission.
    /// An empty list indicates that all providers are allowed.
    /// </summary>
    public List<string> AllowedProviders { get; }

    [JsonIgnore]
    public List<ISimpleStateChecker<PermissionDefinition>> StateCheckers { get; }

    private LocalizedString _displayName;
    public LocalizedString DisplayName
    {
        get => _displayName;
        set => _displayName = value.NotNull(nameof(value));
    }


    private readonly List<PermissionDefinition> _permissions = new();
    public IReadOnlyList<PermissionDefinition> Permissions => _permissions.ToImmutableList();

    private readonly List<PermissionDefinition> _children;
    public IReadOnlyList<PermissionDefinition> Children => _children.ToImmutableList();

    /// <summary>
    /// Can be used to get/set custom properties for this permission definition.
    /// </summary>
    public Dictionary<string, object?> Properties { get; }

    /// <summary>
    /// Indicates whether this permission is enabled or disabled.
    /// A permission is normally enabled.
    /// A disabled permission can not be granted to anyone, but it is still
    /// will be available to check its value (while it will always be false).
    ///
    /// Disabling a permission would be helpful to hide a related application
    /// functionality from users/clients.
    ///
    /// Default: true.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets/sets a key-value on the <see cref="Properties"/>.
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <returns>
    /// Returns the value in the <see cref="Properties"/> dictionary by given <paramref name="name"/>.
    /// Returns null if given <paramref name="name"/> is not present in the <see cref="Properties"/> dictionary.
    /// </returns>
    public object? this[string name]
    {
        get => Properties.GetOrDefault(name);
        set => Properties[name] = value;
    }

    protected internal PermissionDefinition(string name, LocalizedString? displayName = null, bool isEnabled = true)
    {
        Name = name;
        IsEnabled = isEnabled;
        AllowedProviders = new List<string>();
        Properties = new Dictionary<string, object?>();
        StateCheckers = new List<ISimpleStateChecker<PermissionDefinition>>();
        _children = new List<PermissionDefinition>();
        _displayName = displayName ?? new LocalizedString(name, name);
    }


    public PermissionDefinition AddPermission(string name, LocalizedString? displayName = null, bool isEnabled = true)
    {
        var permission = new PermissionDefinition(name, displayName, isEnabled);

        _permissions.Add(permission);

        return permission;
    }

    public virtual PermissionDefinition AddChild(string name, LocalizedString? displayName = null, bool isEnabled = true)
    {
        var child = new PermissionDefinition(name, displayName, isEnabled)
        {
            Parent = this
        };

        _children.Add(child);

        return child;
    }

    /// <summary>
    /// Sets a property in the <see cref="Properties"/> dictionary.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual PermissionDefinition WithProperty(string key, object value)
    {
        Properties[key] = value;
        return this;
    }

    /// <summary>
    /// Set the <see cref="StateProviders"/> property.
    /// This is a shortcut for nested calls on this object.
    /// </summary>
    public virtual PermissionDefinition WithProviders(params string[] providers)
    {
        if (!providers.IsNullOrEmpty())
        {
            AllowedProviders.AddRange(providers);
        }

        return this;
    }

    public override string ToString()
    {
        return $"[{nameof(PermissionDefinition)} {Name}]";
    }
}
