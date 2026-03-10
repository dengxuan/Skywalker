using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Interface for managing localization texts in the database.
/// </summary>
public interface ILocalizationTextManager
{
    /// <summary>
    /// Gets a localization text by its key.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The localization text, or null if not found.</returns>
    Task<LocalizationText?> GetAsync(string resourceName, string cultureName, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all localization texts for a resource and culture.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of localization texts.</returns>
    Task<List<LocalizationText>> GetListAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all localization texts for a resource.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of localization texts.</returns>
    Task<List<LocalizationText>> GetListByResourceAsync(string resourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates a localization text.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="value">The localized value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or updated localization text.</returns>
    Task<LocalizationText> SetAsync(string resourceName, string cultureName, string key, string value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a localization text.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="key">The localization key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(string resourceName, string cultureName, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all localization texts for a resource and culture.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted texts.</returns>
    Task<int> DeleteByCultureAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports localization texts from a dictionary.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="texts">The dictionary of key-value pairs to import.</param>
    /// <param name="overwrite">Whether to overwrite existing texts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of imported texts.</returns>
    Task<int> ImportAsync(string resourceName, string cultureName, Dictionary<string, string> texts, bool overwrite = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports localization texts to a dictionary.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A dictionary of key-value pairs.</returns>
    Task<Dictionary<string, string>> ExportAsync(string resourceName, string cultureName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all supported cultures for a resource.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of culture names.</returns>
    Task<List<string>> GetSupportedCulturesAsync(string resourceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cache for a resource and optionally a specific culture.
    /// </summary>
    /// <param name="resourceName">The resource name.</param>
    /// <param name="cultureName">The culture name, or null to invalidate all cultures.</param>
    void InvalidateCache(string resourceName, string? cultureName = null);
}

