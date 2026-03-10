using System.Globalization;

namespace Skywalker.Localization;

/// <summary>
/// Provides localized strings for a specific resource.
/// </summary>
public interface IStringLocalizer
{
    /// <summary>
    /// Gets the localized string for the specified name.
    /// </summary>
    /// <param name="name">The name of the localized string.</param>
    /// <returns>The localized string.</returns>
    LocalizedString this[string name] { get; }

    /// <summary>
    /// Gets the localized string for the specified name with arguments.
    /// </summary>
    /// <param name="name">The name of the localized string.</param>
    /// <param name="arguments">The arguments to format the string with.</param>
    /// <returns>The localized string.</returns>
    LocalizedString this[string name, params object[] arguments] { get; }

    /// <summary>
    /// Gets all localized strings for the current culture.
    /// </summary>
    /// <param name="includeParentCultures">Whether to include strings from parent cultures.</param>
    /// <returns>All localized strings.</returns>
    IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures = true);

    /// <summary>
    /// Gets all localized strings for the specified culture.
    /// </summary>
    /// <param name="culture">The culture to get strings for.</param>
    /// <param name="includeParentCultures">Whether to include strings from parent cultures.</param>
    /// <returns>All localized strings.</returns>
    IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeParentCultures = true);
}

/// <summary>
/// Provides localized strings for a specific resource type.
/// </summary>
/// <typeparam name="TResource">The resource type.</typeparam>
public interface IStringLocalizer<TResource> : IStringLocalizer
{
}

