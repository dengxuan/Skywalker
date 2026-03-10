// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Settings.EntityFrameworkCore.Entities;

/// <summary>
/// Setting entity for storing configuration values.
/// Uses composite key: (Name, ProviderName, ProviderKey).
/// </summary>
public class Setting : Entity
{
    /// <summary>
    /// Setting name.
    /// </summary>
    public virtual string Name { get; protected set; } = null!;

    /// <summary>
    /// Setting value.
    /// </summary>
    public virtual string? Value { get; internal set; }

    /// <summary>
    /// Provider name (e.g., "G" for Global, "U" for User).
    /// </summary>
    public virtual string ProviderName { get; protected set; } = null!;

    /// <summary>
    /// Provider key (e.g., user ID). Empty string for global settings.
    /// </summary>
    public virtual string ProviderKey { get; protected set; } = string.Empty;

    /// <summary>
    /// Protected constructor for EF Core.
    /// </summary>
    protected Setting() { }

    /// <summary>
    /// Creates a new Setting instance.
    /// </summary>
    public Setting(string name, string value, string providerName, string? providerKey = null)
    {
        Check.NotNull(name, nameof(name));
        Check.NotNull(value, nameof(value));
        Check.NotNull(providerName, nameof(providerName));

        Name = name;
        Value = value;
        ProviderName = providerName;
        ProviderKey = providerKey ?? string.Empty;
    }

    /// <summary>
    /// Sets the value of this setting.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void SetValue(string value)
    {
        Check.NotNull(value, nameof(value));
        Value = value;
    }

    /// <inheritdoc/>
    public override object[] GetKeys() => [Name, ProviderName, ProviderKey];

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[Setting] Name = {Name}, ProviderName = {ProviderName}, ProviderKey = {ProviderKey}, Value = {Value}";
    }
}
