using Microsoft.Extensions.Options;

namespace Skywalker.Localization.AspNetCore;

/// <summary>
/// Default implementation of <see cref="ILanguageProvider"/> that reads from <see cref="LocalizationOptions"/>.
/// </summary>
public class DefaultLanguageProvider : ILanguageProvider
{
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Creates a new instance of <see cref="DefaultLanguageProvider"/>.
    /// </summary>
    public DefaultLanguageProvider(IOptions<LocalizationOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
        return Task.FromResult<IReadOnlyList<LanguageInfo>>(_options.Languages);
    }
}

