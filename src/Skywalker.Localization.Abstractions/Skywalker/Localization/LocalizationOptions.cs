namespace Skywalker.Localization;

/// <summary>
/// Options for configuring localization.
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// Gets the dictionary of localization resources.
    /// </summary>
    public LocalizationResourceDictionary Resources { get; }

    /// <summary>
    /// Gets or sets the default resource type to use when no specific resource is specified.
    /// </summary>
    public Type? DefaultResourceType { get; set; }

    /// <summary>
    /// Gets the list of global contributors that apply to all resources.
    /// </summary>
    public List<ILocalizationResourceContributor> GlobalContributors { get; }

    /// <summary>
    /// Gets the list of languages supported by the application.
    /// </summary>
    public List<LanguageInfo> Languages { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LocalizationOptions"/>.
    /// </summary>
    public LocalizationOptions()
    {
        Resources = new LocalizationResourceDictionary();
        GlobalContributors = new List<ILocalizationResourceContributor>();
        Languages = new List<LanguageInfo>();
    }
}

