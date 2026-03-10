namespace Skywalker.Localization;

/// <summary>
/// Represents information about a supported language.
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// Gets the culture name (e.g., "en", "en-US", "zh-CN").
    /// </summary>
    public string CultureName { get; }

    /// <summary>
    /// Gets the UI culture name. If not specified, defaults to <see cref="CultureName"/>.
    /// </summary>
    public string UiCultureName { get; }

    /// <summary>
    /// Gets the display name of the language.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets or sets the flag icon for the language (e.g., a CSS class or image path).
    /// </summary>
    public string? FlagIcon { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the default language.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this language is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/>.
    /// </summary>
    /// <param name="cultureName">The culture name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="uiCultureName">The UI culture name (defaults to cultureName if not specified).</param>
    /// <param name="flagIcon">The flag icon.</param>
    /// <param name="isDefault">Whether this is the default language.</param>
    public LanguageInfo(
        string cultureName,
        string displayName,
        string? uiCultureName = null,
        string? flagIcon = null,
        bool isDefault = false)
    {
        CultureName = cultureName ?? throw new ArgumentNullException(nameof(cultureName));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        UiCultureName = uiCultureName ?? cultureName;
        FlagIcon = flagIcon;
        IsDefault = isDefault;
    }
}

