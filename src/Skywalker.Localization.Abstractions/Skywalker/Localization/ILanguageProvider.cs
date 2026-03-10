namespace Skywalker.Localization;

/// <summary>
/// Provides available languages for the application.
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// Gets the list of available languages.
    /// </summary>
    /// <returns>A task that returns the list of available languages.</returns>
    Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync();
}

