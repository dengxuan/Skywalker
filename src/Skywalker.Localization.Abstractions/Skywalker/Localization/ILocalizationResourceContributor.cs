namespace Skywalker.Localization;

/// <summary>
/// Contributes localized strings to a localization resource.
/// </summary>
public interface ILocalizationResourceContributor
{
    /// <summary>
    /// Initializes the contributor with the given context.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    void Initialize(LocalizationResourceInitializationContext context);

    /// <summary>
    /// Gets a localized string for the given name and culture.
    /// </summary>
    /// <param name="cultureName">The culture name (e.g., "en-US", "zh-CN").</param>
    /// <param name="name">The name of the localized string.</param>
    /// <returns>The localized string, or null if not found.</returns>
    LocalizedString? GetOrNull(string cultureName, string name);

    /// <summary>
    /// Fills the dictionary with all localized strings for the given culture.
    /// </summary>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="dictionary">The dictionary to fill.</param>
    void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary);

    /// <summary>
    /// Gets all supported culture names.
    /// </summary>
    /// <returns>A list of supported culture names.</returns>
    IEnumerable<string> GetSupportedCultures();
}

