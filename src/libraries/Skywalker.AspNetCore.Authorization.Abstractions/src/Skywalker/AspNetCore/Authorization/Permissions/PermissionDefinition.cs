using System.Collections.Immutable;
using Microsoft.Extensions.Localization;
using Skywalker.Extensions.SimpleStateChecking;

namespace Skywalker.AspNetCore.Authorization.Permissions;

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
    public PermissionDefinition? Parent { get; private set; }

    /// <summary>
    /// A list of allowed providers to get/set value of this permission.
    /// An empty list indicates that all providers are allowed.
    /// </summary>
    public List<string> Providers { get; } //TODO: Rename to AllowedProviders?

    public List<ISimpleStateChecker<PermissionDefinition>> StateCheckers { get; }

    public LocalizedString DisplayName
    {
        get => _displayName;
        set => _displayName = value.NotNull(nameof(value));
    }
    private LocalizedString _displayName;

    public IReadOnlyList<PermissionDefinition> Children => _children.ToImmutableList();
    private readonly List<PermissionDefinition> _children;

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
        Name = name.NotNull(nameof(name));
        _displayName = displayName ?? new LocalizedString(name, name);
        IsEnabled = isEnabled;

        Properties = new Dictionary<string, object?>();
        Providers = new List<string>();
        StateCheckers = new List<ISimpleStateChecker<PermissionDefinition>>();
        _children = new List<PermissionDefinition>();
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
            Providers.AddRange(providers);
        }

        return this;
    }

    public override string ToString()
    {
        return $"[{nameof(PermissionDefinition)} {Name}]";
    }
}
