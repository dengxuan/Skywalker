﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
#if NETCOREAPP3_1_OR_GREATER
using System.Text.Json.Serialization;
#endif
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
    public PermissionDefinition? Parent { get; set; }

    /// <summary>
    /// A list of allowed providers to get/set value of this permission.
    /// An empty list indicates that all providers are allowed.
    /// </summary>
    public string[] AllowedProviders { get; }

    public List<ISimpleStateChecker<PermissionDefinition>> StateCheckers { get; } = new();

    public string DisplayName { get; }

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

    public PermissionDefinition(string name, string displayName, bool isEnabled = true, Dictionary<string, object?>? properties = null, string[]? allowedProviders = null)
    {
        Name = name;
        DisplayName = displayName;
        IsEnabled = isEnabled;
        Properties = properties ?? new Dictionary<string, object?>();
        AllowedProviders = allowedProviders ?? Array.Empty<string>();
        _children = new List<PermissionDefinition>();
    }

    public virtual PermissionDefinition AddChild(string name, string displayName, bool isEnabled = true)
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
            AllowedProviders.AddIfNotContains(providers);
        }

        return this;
    }

    public override string ToString()
    {
        return $"[{nameof(PermissionDefinition)} {Name}]";
    }
}
