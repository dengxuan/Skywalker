using Microsoft.Extensions.Options;

namespace Skywalker.Localization;

public class DefaultLanguageProvider : ILanguageProvider/*, ITransientDependency*/
{
    protected SkywalkerLocalizationOptions Options { get; }

    public DefaultLanguageProvider(IOptions<SkywalkerLocalizationOptions> options)
    {
        Options = options.Value;
    }

    public Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
        return Task.FromResult((IReadOnlyList<LanguageInfo>)Options.Languages);
    }
}
