// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Settings.Abstractions;

/// <summary>
/// Interface for managing settings (write operations).
/// </summary>
public interface ISettingManager
{
    /// <summary>
    /// Sets the value of a setting for the specified provider.
    /// </summary>
    /// <param name="name">The name of the setting.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="providerName">The provider name (e.g., "G" for global, "U" for user).</param>
    /// <param name="providerKey">The provider key (e.g., user ID). Null for global settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task SetAsync(string name, string value, string providerName, string? providerKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a setting for the specified provider.
    /// When deleted, the setting will fallback to the next provider in the chain.
    /// </summary>
    /// <param name="name">The name of the setting.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="providerKey">The provider key. Null for global settings.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task DeleteAsync(string name, string providerName, string? providerKey = null, CancellationToken cancellationToken = default);
}
